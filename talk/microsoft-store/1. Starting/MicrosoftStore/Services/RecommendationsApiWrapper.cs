namespace Recommendations
{
    using System;
    using System.IO;
    using System.Net.Http;
    using Newtonsoft.Json;
    using System.Net.Http.Formatting;
    using System.Linq;
    using System.Threading;
    

    /// <summary>
    /// A wrapper class to invoke Recommendations REST APIs
    /// </summary>
    public class RecommendationsApi
    {
        private readonly HttpClient _httpClient;
        public string BaseUri;

        /// <summary>
        /// Constructor that initializes the Http Client.
        /// </summary>
        /// <param name="accountKey">The account key</param>
		public RecommendationsApi(string accountKey, string baseUri)
        {
            BaseUri = baseUri;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUri),
                Timeout = TimeSpan.FromMinutes(5),
                DefaultRequestHeaders =
                {
                    {"Ocp-Apim-Subscription-Key", accountKey}
                }
            };
        }

        /// <summary>
        /// Set an active build for the model.
        /// </summary>
        /// <param name="modelId">Unique idenfier of the model</param>
        /// <param name="updateActiveBuildInfo"></param>
        public void SetActiveBuild(string modelId, UpdateActiveBuildInfo updateActiveBuildInfo)
        {
            string uri = BaseUri + "/models/" + modelId;
            var content = new ObjectContent<UpdateActiveBuildInfo>(updateActiveBuildInfo, new JsonMediaTypeFormatter());
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), uri) { Content = content };
            var response = _httpClient.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error HTTP Status Code");
            }
        }

        /// <summary>
        /// Get Item to Item (I2I) Recommendations or Frequently-Bought-Together (FBT) recommendations
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        /// <param name="buildId">The build identifier.</param>
        /// <param name="itemIds"></param>
        /// <param name="numberOfResults"></param>
        /// <returns>
        /// The recommendation sets. Note that I2I builds will only return one item per set.
        /// FBT builds will return more than one item per set.
        /// </returns>
        public RecommendedItemSetInfoList GetRecommendations(string modelId, long buildId, string itemIds, int numberOfResults)
        {
            string uri = BaseUri + "/models/" + modelId + "/recommend/item?itemIds=" + itemIds + "&numberOfResults=" + numberOfResults + "&minimalScore=0";
            var response = _httpClient.GetAsync(uri).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    String.Format("Error {0}: Failed to get recommendations for modelId {1}, buildId {2}, Reason: {3}",
                    response.StatusCode, modelId, buildId, ExtractErrorInfo(response)));
            }

            var jsonString = response.Content.ReadAsStringAsync().Result;
            var recommendedItemSetInfoList = JsonConvert.DeserializeObject<RecommendedItemSetInfoList>(jsonString);
            return recommendedItemSetInfoList;
        }

        /// <summary>
        /// Use historical transaction data to provide personalized recommendations for a user.
        /// The user history is extracted from the usage files used to train the model.
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        /// <param name="buildId">The build identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="numberOfResults">Desired number of recommendation results.</param>
        /// <returns>The recommendations for the user.</returns>
        public RecommendedItemSetInfoList GetUserRecommendations(string modelId, long buildId, string userId, int numberOfResults)
        {
            string uri = BaseUri + "/models/" + modelId + "/recommend/user?userId=" + userId + "&numberOfResults=" + numberOfResults;
            var response = _httpClient.GetAsync(uri).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    String.Format("Error {0}: Failed to get user recommendations for modelId {1}, buildId {2}, Reason: {3}",
                    response.StatusCode, modelId, buildId, ExtractErrorInfo(response)));
            }

            var jsonString = response.Content.ReadAsStringAsync().Result;
            var recommendedItemSetInfoList = JsonConvert.DeserializeObject<RecommendedItemSetInfoList>(jsonString);
            return recommendedItemSetInfoList;
        }

        /// <summary>
        /// Update model information
        /// </summary>
        /// <param name="modelId">the id of the model</param>
        /// <param name="description">the model description (optional)</param>
        /// <param name="activeBuildId">the id of the build to be active (optional)</param>
        public void SetActiveBuild(string modelId, long activeBuildId)
        {
            var info = new UpdateActiveBuildInfo()
            {
                ActiveBuildId = activeBuildId
            };

            SetActiveBuild(modelId, info);
        }


        /// <summary>
        /// Extract error message from the httpResponse, (reason phrase + body)
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static string ExtractErrorInfo(HttpResponseMessage response)
        {
            string detailedReason = null;
            if (response.Content != null)
            {
                detailedReason = response.Content.ReadAsStringAsync().Result;
            }
            var errorMsg = detailedReason == null ? response.ReasonPhrase : response.ReasonPhrase + "->" + detailedReason;
            return errorMsg;

        }

        /// <summary>
        /// Extract error information from HTTP response message.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        internal static string ExtractReponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
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

    /// <summary>
    /// Utility class holding the result of import operation
    /// </summary>
    internal class ImportReport
    {
        public string Info { get; set; }
        public int ErrorCount { get; set; }
        public int LineCount { get; set; }

        public override string ToString()
        {
            return string.Format("successfully imported {0}/{1} lines for {2}", LineCount - ErrorCount, LineCount,
                Info);
        }
    }
}
