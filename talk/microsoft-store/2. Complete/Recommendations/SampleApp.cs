/**************************************************************************************************
 *
 * This sample shows how to use the Recommendations API. You can find more details on the 
 * Recommendations API and other Cognitive Services at http://go.microsoft.com/fwlink/?LinkID=759709.  
 * 
 * The Recommendations API identifies consumption patterns from your transaction information 
 * in order to provide recommendations. These recommendations can help your customers more 
 * easily discover items that they may be interested in.  By showing your customers products that 
 * they are more likely to be interested in, you will, in turn, increase your sales.
 * 
 *  Before you run the application:
 *  1. Sign up for the Recommendations API service and get an API Key.
 *     (http://go.microsoft.com/fwlink/?LinkId=761106 )
 *     
 *  2. Set the AccountKey variable in the RecommendationsSampleApp to the key you got.
 *  
 *  3. Verify the endpoint Uri you got when you subscribed matches the BaseUri as it may 
 *     be different if you selected a different data center.
 *  
 *************************************************************************************************/
namespace Recommendations
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    public class RecommendationsSampleApp
    {
        private const string AccountKey = "799bcb7dd81a492286193ce3209ec0f6"; // Enter your API key here.  
        private const string ModelName = "MSStore";
        public const string BaseUri = "https://westus.api.cognitive.microsoft.com/recommendations/v4.0";

        /// <summary>
        /// 1) Builds a recommendations model and upload catalog and usage data
        /// 2) Triggers a model build and monitor the build operation status
        /// 3) Sets the build as the active build for the model.
        /// 4) Requests item recommendations
        /// 5) Requests user recommendations
        /// </summary>
        public static void Main(string[] args)
        {
            // Initialize helper with username and API key.
            var recommender = new RecommendationsApiWrapper(AccountKey, BaseUri);
            string modelId = string.Empty;
            try
            {
                if (String.IsNullOrEmpty(AccountKey))
                {
                    Console.WriteLine("Please enter your API key to run this sample.");
                    Console.ReadKey();
                    return;
                }

				modelId = "a2d93304-457c-4c6c-9a23-4320a58fbe27";
				long buildId = 1558644;
				var itemId = "FKF-00908";

				var recommendations = recommender.GetRecommendations(modelId, buildId, itemId, 3);
				foreach (var rec in recommendations.RecommendedItemSetInfo)
				{
					foreach (var item in rec.Items)
					{
						Console.WriteLine("Item id: {0} \n Item name: {1} \t (Rating  {2})", item.Id, item.Name, rec.Rating);
					}
				}

    //            // Create a new model.
    //            Console.WriteLine("Creating a new model {0}...", ModelName);
    //            ModelInfo modelInfo = recommender.CreateModel(ModelName, "Sample model");
    //            modelId = modelInfo.Id;
    //            Console.WriteLine("Model '{0}' created with ID: {1}", ModelName, modelId);

    //            // Import data to the model.            
    //            var resourcesDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources");
    //            Console.WriteLine("Importing catalog files...");
				//foreach (string catalog in Directory.GetFiles(resourcesDir, "catalog*.csv"))
    //            {
    //                var catalogFile = new FileInfo(catalog);
    //                recommender.UploadCatalog(modelId, catalogFile.FullName, catalogFile.Name);
    //            }

    //            Console.WriteLine("Importing usage data...");
    //            foreach (string usage in Directory.GetFiles(resourcesDir, "usage*.csv"))
    //            {
    //                var usageFile = new FileInfo(usage);
    //                recommender.UploadUsage(modelId, usageFile.FullName, usageFile.Name);
    //            }


    //            // Trigger a recommendation build.
    //            string operationLocationHeader;
    //            Console.WriteLine("Triggering build for model '{0}'. \nThis will take a few minutes...", modelId);
    //            var buildId = recommender.CreateRecommendationsBuild(modelId, "build of " + DateTime.UtcNow.ToString("yyyyMMddHHmmss"), 
    //                                                                 enableModelInsights: false,
    //                                                                 operationLocationHeader: out operationLocationHeader);

    //            // Monitor the build and wait for completion.
    //            Console.WriteLine("Monitoring build {0}", buildId);
    //            var buildInfo = recommender.WaitForBuildCompletion(operationLocationHeader);
    //            Console.WriteLine("Build {0} ended with status {1}.\n", buildId, buildInfo.Status);

    //            if (String.Compare(buildInfo.Status, "Succeeded", StringComparison.OrdinalIgnoreCase) != 0)
    //            {
    //                Console.WriteLine("Build {0} did not end successfully, the sample app will stop here.", buildId);
    //                Console.WriteLine("Press any key to end");
    //                Console.ReadKey();
    //                return;
    //            }

    //            // Waiting  in order to propagate the model updates from the build...
    //            Console.WriteLine("Waiting for 40 sec for propagation of the built model...");
    //            Thread.Sleep(TimeSpan.FromSeconds(40));

                // The below api is more meaningful when you want to give a certain build id to be an active build.
                // Currently this app has a single build which is already active.
                Console.WriteLine("Setting build {0} as active build.", buildId);
                recommender.SetActiveBuild(modelId, buildId);

                // Now we are ready to get recommendations!

                // Get item to item recommendations. (I2I)
                Console.WriteLine();
                Console.WriteLine("Getting Item to Item Recommendations for The Piano Man's Daughter");
                const string itemIds = "6485200";
                var itemSets = recommender.GetRecommendations(modelId, buildId, itemIds, 6);
                if (itemSets.RecommendedItemSetInfo != null)
                {
                    foreach (RecommendedItemSetInfo recoSet in itemSets.RecommendedItemSetInfo)
                    {
                        foreach (var item in recoSet.Items)
                        {
                            Console.WriteLine("Item id: {0} \n Item name: {1} \t (Rating  {2})", item.Id, item.Name, recoSet.Rating);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No recommendations found.");
                }

                // Now let's get a user recommendation (U2I)
                Console.WriteLine();
                Console.WriteLine("Getting User Recommendations for User:");
                string userId = "142256";
                itemSets = recommender.GetUserRecommendations(modelId, buildId, userId, 6);
                if (itemSets.RecommendedItemSetInfo != null)
                {
                    foreach (RecommendedItemSetInfo recoSet in itemSets.RecommendedItemSetInfo)
                    {
                        foreach (var item in recoSet.Items)
                        {
                            Console.WriteLine("Item id: {0} \n Item name: {1} \t (Rating  {2})", item.Id, item.Name, recoSet.Rating);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No recommendations found.");
                }

                Console.WriteLine("Press any key to end");
                Console.ReadKey();
            }
            finally
            {
                // Uncomment the line below if you wish to delete the model.
                // Note that you can have up to 10 models at any time. 
                // You may have up to 20 builds per model.
                //recommender.DeleteModel(modelId); 
            }
        }
    }
}