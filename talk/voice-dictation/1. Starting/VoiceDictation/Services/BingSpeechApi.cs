using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Threading;

using Newtonsoft.Json;

namespace VoiceDictation
{
	[DataContract]
	public class AccessTokenInfo
	{
		[DataMember]
		public string access_token { get; set; }
		[DataMember]
		public string token_type { get; set; }
		[DataMember]
		public string expires_in { get; set; }
		[DataMember]
		public string scope { get; set; }
	}

	/*
     * This class demonstrates how to get a valid O-auth token.
     */
	public class Authentication
	{
		public static readonly string AccessUri = "https://oxford-speech.cloudapp.net/token/issueToken";
		private string clientId;
		private string clientSecret;
		private string request;
		private AccessTokenInfo token;
		private Timer accessTokenRenewer;

		//Access token expires every 10 minutes. Renew it every 9 minutes only.
		private const int RefreshTokenDuration = 9;

		public Authentication(string clientId, string clientSecret)
		{
			this.clientId = clientId;
			this.clientSecret = clientSecret;

			/*
             * If clientid or client secret has special characters, encode before sending request
             */
			this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope={2}",
										  HttpUtility.UrlEncode(clientId),
										  HttpUtility.UrlEncode(clientSecret),
										  HttpUtility.UrlEncode("https://speech.platform.bing.com"));

			this.token = HttpPost(AccessUri, this.request);

			// renew the token every specfied minutes
			accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback),
										   this,
										   TimeSpan.FromMinutes(RefreshTokenDuration),
										   TimeSpan.FromMilliseconds(-1));
		}

		public AccessTokenInfo GetAccessToken()
		{
			return this.token;
		}

		private void RenewAccessToken()
		{
			AccessTokenInfo newAccessToken = HttpPost(AccessUri, this.request);
			//swap the new token with old one
			//Note: the swap is thread unsafe
			this.token = newAccessToken;
			Console.WriteLine(string.Format("Renewed token for user: {0} is: {1}",
							  this.clientId,
							  this.token.access_token));
		}

		private void OnTokenExpiredCallback(object stateInfo)
		{
			try
			{
				RenewAccessToken();
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
			}
			finally
			{
				try
				{
					accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
				}
				catch (Exception ex)
				{
					Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
				}
			}
		}

		private AccessTokenInfo HttpPost(string accessUri, string requestDetails)
		{
			//Prepare OAuth request 
			WebRequest webRequest = WebRequest.Create(accessUri);
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.Method = "POST";
			byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
			webRequest.ContentLength = bytes.Length;
			using (Stream outputStream = webRequest.GetRequestStream())
			{
				outputStream.Write(bytes, 0, bytes.Length);
			}
			using (WebResponse webResponse = webRequest.GetResponse())
			{
				DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AccessTokenInfo));
				//Get deserialized object from JSON stream
				AccessTokenInfo token = (AccessTokenInfo)serializer.ReadObject(webResponse.GetResponseStream());
				return token;
			}
		}
	}

	public class BingSpeechApi
	{
		AccessTokenInfo token;
		string headerValue;

		public static async Task<SpeechResult> SpeechToTextAsync(string appId, string key)
		{
			return await Task.Run(() => SpeechToText(appId, key));
		}

		public static SpeechResult SpeechToText(string appId, string key)
		{
			AccessTokenInfo token;
			string headerValue;

			Authentication auth = new Authentication(appId, key);

			string requestUri = "https://speech.platform.bing.com/recognize";

			/* URI Params. Refer to the README file for more information. */
			requestUri += @"?scenarios=smd";                                  // websearch is the other main option.
			requestUri += @"&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5";     // You must use this ID.
			requestUri += @"&locale=en-US";                                   // We support several other languages.  Refer to README file.
			requestUri += @"&device.os=wp7";
			requestUri += @"&version=3.0";
			requestUri += @"&format=json";
			requestUri += @"&instanceid=565D69FF-E928-4B7E-87DA-9A750B96D9E3";
			requestUri += @"&requestid=" + Guid.NewGuid().ToString();

			string host = @"speech.platform.bing.com";
			string contentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";

			/*
             * Input your own audio file or use read from a microphone stream directly.
             */
			var localStorage = PCLStorage.FileSystem.Current.LocalStorage.Path;
			string audioFile = localStorage + "/SmartCoffee.wav";
			string responseString;
			FileStream fs = null;

#if __ANDROID__
			var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			audioFile = System.IO.Path.Combine(path, "SmartCoffee.wav");
#endif

			token = auth.GetAccessToken();
			Console.WriteLine("Token: {0}\n", token.access_token);

			/*
			 * Create a header with the access_token property of the returned token
			 */
			headerValue = "Bearer " + token.access_token;

			Console.WriteLine("Request Uri: " + requestUri + Environment.NewLine);

			HttpWebRequest request = null;
			request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
			request.SendChunked = true;
			request.Accept = @"application/json;text/xml";
			request.Method = "POST";
			request.ProtocolVersion = HttpVersion.Version11;
			request.Host = host;
			request.ContentType = contentType;
			request.Headers["Authorization"] = headerValue;

			using (fs = new FileStream(audioFile, FileMode.Open, FileAccess.Read))
			{

				/*
				 * Open a request stream and write 1024 byte chunks in the stream one at a time.
				 */
				byte[] buffer = null;
				int bytesRead = 0;
				using (Stream requestStream = request.GetRequestStream())
				{
					/*
					 * Read 1024 raw bytes from the input audio file.
					 */
					buffer = new Byte[checked((uint)Math.Min(1024, (int)fs.Length))];
					while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
					{
						requestStream.Write(buffer, 0, bytesRead);
					}

					// Flush
					requestStream.Flush();
				}

				/*
				 * Get the response from the service.
				 */
				using (WebResponse response = request.GetResponse())
				{
					Console.WriteLine(((HttpWebResponse)response).StatusCode);

					using (StreamReader sr = new StreamReader(response.GetResponseStream()))
					{
						responseString = sr.ReadToEnd();
					}

					Console.WriteLine(responseString);

					var root = JsonConvert.DeserializeObject<RootObject>(responseString);

					return root.results[0];
				}
			}
		}
	}

	[JsonObject("result")]
	public class SpeechResult
	{
		public string Scenario { get; set; }
		public string Name { get; set; }
		public string Lexical { get; set; }
		public string Confidence { get; set; }
	}

	public class RootObject
	{
		public List<SpeechResult> results { get; set; }
	}
}
