using System.Net.Http.Formatting;

namespace Microsoft.Sage.IntegrationTest
{
    using System.IO;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using Newtonsoft.Json;
    using Microsoft.Sage.Cloud.RecommendationWebRole.Models.Api4.Request;
    using Microsoft.Sage.Cloud.RecommendationWebRole.Models.Api4.Response;

    /// <summary>
    /// Wrapper class to invoke /recommendations/v4.0/ APIs 
    /// </summary>
    public class V4ApiClient
    {
        public V4ApiClient(string baseUri, string userSubscriptionId)
        {
            _baseUri = baseUri;
            _userSubscriptionId = userSubscriptionId;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUri),
                Timeout = TimeSpan.FromHours(1),
                DefaultRequestHeaders =
                {
                    {"Authorization", "Basic Y21sYXByb2RhcGk6cjFBQkxBcUM2enQ0a085"}, // Access key that is expected from Marketplace vNext calls
                    {"apim-subscription-id", userSubscriptionId}
                }
            };
        }

        private readonly string _baseUri;
        private readonly string _userSubscriptionId;
        private readonly HttpClient _httpClient;

        public ModelInfo GetModelById(string id)
        {
            string uri = _baseUri + "/recommendations/v4.0/models/" + id;
            var response = _httpClient.GetAsync(uri).Result;
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var modelInfo = JsonConvert.DeserializeObject<ModelInfo>(jsonString);
            return modelInfo;
        }

        public ModelInfoList GetAllModels()
        {
            string uri = _baseUri + "/recommendations/v4.0/models/";
            var response = _httpClient.GetAsync(uri).Result;
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var modelInfo = JsonConvert.DeserializeObject<ModelInfoList>(jsonString);
            return modelInfo;
        }

        public ModelInfo CreateModel(string modelName, string description = null)
        {
            string uri = _baseUri + "/recommendations/v4.0/models/";
            var modelRequestInfo = new ModelRequestInfo { ModelName = modelName, Description = description };
            var response = _httpClient.PostAsJsonAsync(uri, modelRequestInfo).Result;
            var jsonString = ExtractReponse(response);
            var modelInfo = JsonConvert.DeserializeObject<ModelInfo>(jsonString);
            return modelInfo;
        }

        public void DeleteModel(string id)
        {
            string uri = _baseUri + "/recommendations/v4.0/models/" + id;
            var response = _httpClient.DeleteAsync(uri).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error HTTP Status Code");
            }
        }

        public CatalogImportStats ImportCatalog(string modelId, string filePath)
        {
            var filestream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var fileName = Path.GetFileName(filePath);

            string uri = _baseUri + "/recommendations/v4.0/models/" + modelId + "/catalog?fileName=" + fileName;

            var response = _httpClient.PostAsync(uri, new StreamContent(filestream)).Result;

            var jsonString = ExtractReponse(response);
            var catalogImportStats = JsonConvert.DeserializeObject<CatalogImportStats>(jsonString);
            return catalogImportStats;
        }

        public UsageImportStats ImportUsage(string modelId, string filePath)
        {
            var filestream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var fileName = Path.GetFileName(filePath);

            string uri = _baseUri + "/recommendations/v4.0/models/" + modelId + "/usage?fileName=" + fileName;

            var response = _httpClient.PostAsync(uri, new StreamContent(filestream)).Result;

            var jsonString = ExtractReponse(response);
            var usageImportStats = JsonConvert.DeserializeObject<UsageImportStats>(jsonString);
            return usageImportStats;
        }

        public long BuildModel(string modelId, BuildRequestInfo buildRequestInfo, out string operationLocationHeader)
        {
            string uri = _baseUri + "/recommendations/v4.0/models/" + modelId + "/builds";
            var response = _httpClient.PostAsJsonAsync(uri, buildRequestInfo).Result;
            var jsonString = response.Content.ReadAsStringAsync().Result;
            operationLocationHeader = response.Headers.GetValues("Operation-Location").FirstOrDefault();
            var buildModelResponse = JsonConvert.DeserializeObject<BuildModelResponse>(jsonString);
            return buildModelResponse.BuildId;
        }

        public void SetActiveBuild(string modelId, UpdateModelRequestInfo updateModelRequestInfo)
        {
            string uri = _baseUri + "/recommendations/v4.0/models/" + modelId;
            var content = new ObjectContent<UpdateModelRequestInfo>(updateModelRequestInfo, new JsonMediaTypeFormatter());
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), uri) { Content = content };
            var response = _httpClient.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error HTTP Status Code");
            }
        }

        public BuildInfo GetBuildById(string modelId, long buildId)
        {
            string uri = _baseUri + "/recommendations/v4.0/models/" + modelId + "/builds/" + buildId;
            var response = _httpClient.GetAsync(uri).Result;
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var buildInfo = JsonConvert.DeserializeObject<BuildInfo>(jsonString);
            return buildInfo;
        }


        public BuildInfoList GetAllBuilds(string modelId)
        {
            string uri = _baseUri + "/recommendations/v4.0/models/" + modelId + "/builds";
            var response = _httpClient.GetAsync(uri).Result;
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var buildInfo = JsonConvert.DeserializeObject<BuildInfoList>(jsonString);
            return buildInfo;
        }

        public void DeleteBuild(string modelId, long buildId)
        {
            string uri = _baseUri + "/recommendations/v4.0/models/" + modelId + "/builds/" + buildId;
            var response = _httpClient.DeleteAsync(uri).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error HTTP Status Code");
            }
        }

        public RecommendedItemSetInfoList GetItemRecommendation(string modelId, long buildId, string itemIds, int numberOfResults)
        {
            string uri = _baseUri + "/recommendations/v4.0/models/" + modelId + "/recommend/item?itemIds=" + itemIds + "&numberOfResults=" + numberOfResults;
            var response = _httpClient.GetAsync(uri).Result;
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var recommendedItemSetInfoList = JsonConvert.DeserializeObject<RecommendedItemSetInfoList>(jsonString);
            return recommendedItemSetInfoList;
        }

        public RecommendedItemSetInfoList GetUserRecommendation(string modelId, long buildId, string userId, int numberOfResults)
        {
            string uri = _baseUri + "/recommendations/v4.0/models/" + modelId + "/recommend/user?userId=" + userId + "&numberOfResults=" + numberOfResults;
            var response = _httpClient.GetAsync(uri).Result;
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var recommendedItemSetInfoList = JsonConvert.DeserializeObject<RecommendedItemSetInfoList>(jsonString);
            return recommendedItemSetInfoList;
        }

        public OperationInfo<BuildInfo> CheckForBuildCompletion(string locationHeader)
        {
            OperationInfo<BuildInfo> operationInfo;
            while (true)
            {
                var response = _httpClient.GetAsync(locationHeader).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                operationInfo = JsonConvert.DeserializeObject<OperationInfo<BuildInfo>>(jsonString);

                // BuildStatus{Queued,Building,Cancelling,Canceled,Succeded,Failed}

                if (operationInfo.Status.Equals("Succeeded") ||
                    operationInfo.Status.Equals("Failed") ||
                    operationInfo.Status.Equals("Canceled"))
                {
                    break;
                }
                Thread.Sleep(2 * 1000);
            }
            return operationInfo;
        }

        public void CancelBuild(string locationHeader)
        {
            var deleteResponse = _httpClient.DeleteAsync(locationHeader).Result;
            var response = _httpClient.GetAsync(locationHeader).Result;
            var jsonString = response.Content.ReadAsStringAsync().Result;
            var operationInfo = JsonConvert.DeserializeObject<OperationInfo<BuildInfo>>(jsonString);
            Debug.Assert(operationInfo.Status.Equals("Cancelling"));

            while (true)
            {
                response = _httpClient.GetAsync(locationHeader).Result;
                jsonString = response.Content.ReadAsStringAsync().Result;
                operationInfo = JsonConvert.DeserializeObject<OperationInfo<BuildInfo>>(jsonString);
                if (operationInfo.Status.Equals("Cancelled"))
                {
                    break;
                }
                Thread.Sleep(2 * 1000);
            }

            if (!deleteResponse.IsSuccessStatusCode)
            {
                throw new Exception("Error HTTP Status Code");
            }
        }

        private static string ExtractReponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            // Extract the error information
            // DM send the error message in body so need to extract the info from there
            string detailedReason = null;
            if (response.Content != null)
            {
                detailedReason = response.Content.ReadAsStringAsync().Result;
            }
            var errorMsg = detailedReason == null ? response.ReasonPhrase : response.ReasonPhrase + "->" + detailedReason;

            string error = String.Format("Status code: {0}\nDetail information: {1}", (int)response.StatusCode, errorMsg);
            throw new Exception("Response: " + error);            
        }
    }
}
