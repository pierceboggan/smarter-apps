using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Sage.Cloud.RecommendationWebRole.Models.Api4.Request;
using Microsoft.Sage.Common.Build;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Sage.IntegrationTest
{
    [TestClass]
    public class V4ApiDemoTest 
    {
        private static readonly V4ApiClient ApiClient = new V4ApiClient("http://127.0.0.1:8080/", "tempUserId");

        [TestMethod, TestCategory("IntegrationTest")]
        public void CreateModelBasicTest()
        {
            var model = ApiClient.CreateModel("BasicCreateModelTest", "Test Model Description");
            Assert.IsTrue(model.Name.Equals("TestModel", StringComparison.OrdinalIgnoreCase));
            ApiClient.DeleteModel(model.Id);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetModelByIdBasicTest()
        {
            var model = ApiClient.CreateModel("BasicGetModelByIdTest", "Test Model Description");
            var getModel = ApiClient.GetModelById(model.Id);
            Assert.IsTrue(Utils.PublicInstancePropertiesEqual(getModel, model));
            ApiClient.DeleteModel(model.Id);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetAllModelsBasicTest()
        {
            const string modelName1 = "testmodel1";
            var model1 = ApiClient.CreateModel(modelName1);
            const string modelName2 = "testmodel2";
            var model2 = ApiClient.CreateModel(modelName2);

            var modelInfoList = ApiClient.GetAllModels();
            Assert.IsTrue(modelInfoList.Models.ToList().Count >= 2);
            Assert.IsTrue((modelInfoList.Models.ToList().Select(x => x).Where(modelInfo => modelInfo.Name.Equals(modelName1))).Count() == 1);
            Assert.IsTrue((modelInfoList.Models.ToList().Select(x => x).Where(modelInfo => modelInfo.Name.Equals(modelName2))).Count() == 1);
            
            ApiClient.DeleteModel(model1.Id);
            ApiClient.DeleteModel(model2.Id);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        [DeploymentItem(@"..\..\..\..\Tests\Source\APITester\APITester\DataFiles\books-catalog-with-features.csv")]
        [DeploymentItem(@"..\..\..\..\Tests\Source\APITester\APITester\DataFiles\books-usage.csv")]
        public void BuildModelBasicTest()
        {
            const string modelName = "BooksModel";
            string catalogFilePath = Path.Combine(TestContext.DeploymentDirectory, "books-catalog-with-features.csv");
            string usageFilePath = Path.Combine(TestContext.DeploymentDirectory, "books-usage.csv");

            var model = ApiClient.CreateModel(modelName);
            
            var catalogImportStats = ApiClient.ImportCatalog(model.Id, catalogFilePath);
            Assert.IsTrue(catalogImportStats.ProcessedLineCount == catalogImportStats.ImportedLineCount, "Import catalog failure");

            var usageImportStats = ApiClient.ImportUsage(model.Id, usageFilePath);
            Assert.IsTrue(usageImportStats.ProcessedLineCount == usageImportStats.ImportedLineCount, "Import usage failure");

            // Build 1
            var buildRequestInfo1 = new BuildRequestInfo
            {
                Description = "FBT build",
                BuildType = BuildType.Fbt.ToString()
            };

            string operationLocationHeader1;
            var buildId1 = ApiClient.BuildModel(model.Id, buildRequestInfo1, out operationLocationHeader1);
            Assert.IsTrue(buildId1 >= 0, "BuildId not valid");

            var buildInfo1 = ApiClient.CheckForBuildCompletion(operationLocationHeader1);
            Assert.IsTrue(buildInfo1.Status == "Succeeded");

            var getBuildInfo1 = ApiClient.GetBuildById(model.Id, buildId1);
            Assert.IsTrue(getBuildInfo1.Id == buildId1);
            Assert.IsTrue(getBuildInfo1.Type.ToString() == buildRequestInfo1.BuildType);
            //Assert.IsTrue(getBuild.Description == buildRequestInfo.Description);  // BUG: This is a bug in existing API controller!

            // Build 2
            var buildRequestInfo2 = new BuildRequestInfo
            {
                Description = "Recommendation build",
                BuildType = BuildType.Recommendation.ToString()
            };
            string operationLocationHeader2;
            var buildId2 = ApiClient.BuildModel(model.Id, buildRequestInfo2, out operationLocationHeader2);
            Assert.IsTrue(buildId2 >= 0, "BuildId not valid");

            var buildInfo2 = ApiClient.CheckForBuildCompletion(operationLocationHeader2);
            Assert.IsTrue(buildInfo2.Status == "Succeeded");

            ApiClient.SetActiveBuild(model.Id, new UpdateModelRequestInfo { ActiveBuildId = buildId2 });
            var buildInfoList = ApiClient.GetAllBuilds(model.Id);
            Assert.IsTrue(buildInfoList.Builds.ToList().Count == 2);

            //GetItemRecommendation
            const string itemIds = "140282033,140327592";
            var itemSets = ApiClient.GetItemRecommendation(model.Id, buildId2, itemIds, 10);
            Assert.IsTrue(itemSets.RecommendedItemSetInfo.Count() == 10);

            //GetUserRecommendation
            const string userId = "197020";
            var userItemSets = ApiClient.GetUserRecommendation(model.Id, buildId2, userId, 10);
            Assert.IsTrue(itemSets.RecommendedItemSetInfo.Count() == 10);
            
            ApiClient.DeleteBuild(model.Id, buildId1);
            ApiClient.DeleteBuild(model.Id, buildId2);
            ApiClient.DeleteModel(model.Id);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        [DeploymentItem(@"..\..\..\..\Tests\Source\APITester\APITester\DataFiles\books-catalog-with-features.csv")]
        [DeploymentItem(@"..\..\..\..\Tests\Source\APITester\APITester\DataFiles\books-usage.csv")]
        public void CancelBuildBasicTest()
        {
            const string modelName = "testModel";
            string catalogFilePath = Path.Combine(TestContext.DeploymentDirectory, "books-catalog-with-features.csv");
            string usageFilePath = Path.Combine(TestContext.DeploymentDirectory, "books-usage.csv");
            
            var model = ApiClient.CreateModel(modelName);
            var catalogImportStats = ApiClient.ImportCatalog(model.Id, catalogFilePath);
            var usageImportStats = ApiClient.ImportUsage(model.Id, usageFilePath);

            var buildRequestInfo = new BuildRequestInfo
            {
                Description = "Build1",
                BuildType = BuildType.Recommendation.ToString(),
                BuildParameters =
                    new BuildParameters
                    {
                        Recommendation = new RecommendationBuildParameters { NumberOfModelIterations = 10 }
                    }
            };
            string locationHeader;
            var buildId = ApiClient.BuildModel(model.Id, buildRequestInfo, out locationHeader);
            Debug.Assert(buildId != 0, "BuildId not valid");

            ApiClient.CancelBuild(locationHeader);
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }
    }
}
