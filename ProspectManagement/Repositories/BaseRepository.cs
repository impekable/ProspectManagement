using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using static Newtonsoft.Json.JsonConvert;
using static Newtonsoft.Json.JsonTextReader;
using System.Net.Http.Headers;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Models.App;
using Newtonsoft.Json;
using ProspectManagement.Core.Constants;
using Microsoft.AppCenter.Crashes;
using System.Collections.Generic;

namespace ProspectManagement.Core.Repositories
{
    public class BaseRepository : IBaseRepository
    {

        protected const string _e1Uri = Constants.ConnectionURIs.E1Uri;
        static readonly TimeSpan _httpTimeout = TimeSpan.FromSeconds(20);
        static readonly HttpClient _httpClient = CreateHttpClient();
        static readonly HttpClient _httpClientWithAccessToken = CreateHttpClient();
        static readonly HttpClient _httpClientWithUserNamePassword = CreateHttpClient();
        static readonly JsonSerializer _serializer = new JsonSerializer();
        public event EventHandler<RetrievingDataFailureEventArgs> RetrievingDataFailed;

        private static HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient { Timeout = _httpTimeout };
            return httpClient;
        }

        private HttpClient SetAccessTokenHttpClient(string accessToken)
        {
            _httpClientWithAccessToken.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			if (!_httpClientWithAccessToken.DefaultRequestHeaders.Contains("Ocp-Apim-Subscription-Key"))
			{
				_httpClientWithAccessToken.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", PrivateKeys.AzureSubscriptionKey);
			}
            return _httpClientWithAccessToken;
        }

        private HttpClient SetUsernamePasswordHttpClient(string username, string password)
        {
            var authData = string.Format("{0}:{1}", username, password);
            _httpClientWithUserNamePassword.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(authData)));
            return _httpClientWithUserNamePassword;
        }

        protected async Task<T> GetDataObjectFromAPI<T>(string apiUrl, string username, string password)
        {
            return await Task.Run(async () =>
            {
                var client = SetUsernamePasswordHttpClient(username, password);
                try
                {
                    using (var stream = await client.GetStreamAsync(apiUrl).ConfigureAwait(false))
                    using (var reader = new System.IO.StreamReader(stream))
                    using (var json = new JsonTextReader(reader))
                    {
                        if (json == null)
                            return default(T);
                        return _serializer.Deserialize<T>(json);
                    }
                    //var json = await client.GetStringAsync(apiUrl);

                    //if (string.IsNullOrWhiteSpace(json))
                    //    return default(T);
                    //return DeserializeObject<T>(json);
                }
                catch (Exception e)
                {
                    OnRetrievingDataFailed(e.Message);
                    return default(T);
                }
            });
        }

		protected async Task<T> GetDataObjectFromAPI<T>(string apiUrl)
		{
			return await Task.Run(async () =>
			{
				try
				{
					using (var stream = await _httpClient.GetStreamAsync(apiUrl).ConfigureAwait(false))
					using (var reader = new System.IO.StreamReader(stream))
					using (var json = new JsonTextReader(reader))
					{
						if (json == null)
							return default(T);
						return _serializer.Deserialize<T>(json);
					}
					//var json = await _httpClient.GetStringAsync(apiUrl);

					//if (string.IsNullOrWhiteSpace(json))
					//    return default(T);
					//return DeserializeObject<T>(json);
				}
				catch (Exception e)
				{
					OnRetrievingDataFailed(e.Message);
					return default(T);
				}
			});
		}

        protected async Task<String> GetFromAPI(string apiUrl)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    using (var stream = await _httpClient.GetStreamAsync(apiUrl).ConfigureAwait(false))
                    using (var json = new StreamContent(stream))
                    {
                        if (json == null)
                            return null;
                        return await json.ReadAsStringAsync();
                    }
                }
                catch (Exception e)
                {
                    OnRetrievingDataFailed(e.Message);
                    return null;
                }
            });
        }

        protected async Task<T> GetDataObjectFromAPI<T>(string apiUrl, string accessToken)
        {
            return await Task.Run(async () =>
            {
                var client = SetAccessTokenHttpClient(accessToken);
                try
                {
                    using (var stream = await client.GetStreamAsync(apiUrl).ConfigureAwait(false))
                    using (var reader = new System.IO.StreamReader(stream))
                    using (var json = new JsonTextReader(reader))
                    {
                        if (json == null)
                            return default(T);
                        return _serializer.Deserialize<T>(json);
                    }
                    //var json = await _httpClient.GetStringAsync(apiUrl);

                    //if (string.IsNullOrWhiteSpace(json))
                    //    return default(T);
                    //return DeserializeObject<T>(json);
                }
                catch (Exception e)
                {
                    OnRetrievingDataFailed(e.Message);
                    return default(T);
                }
            });
        }

        protected async Task<bool> GetResultFromAPI(string apiUrl, string username, string password)
        {
            return await Task.Run(async () =>
            {
                var client = SetUsernamePasswordHttpClient(username, password);
                try
                {
                    var response = await client.GetAsync(apiUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        try
                        {                            
                            using (var stream = await response.Content.ReadAsStreamAsync())
                            using (var reader = new System.IO.StreamReader(stream))
                            using (var json = new JsonTextReader(reader))
                            {
                                if (json == null)
                                    return false;
                                var error = _serializer.Deserialize<ServiceError>(json);
                                OnRetrievingDataFailed(error.ErrorDescription);
                            }
                            //var contentNew = await response.Content.ReadAsStringAsync();
                            //var error = DeserializeObject<ServiceError>(contentNew);
                            //OnRetrievingDataFailed(error.ErrorDescription);
                        }
                        catch (Exception e)
                        {
                            OnRetrievingDataFailed("Error occurred. " + response.ReasonPhrase);
                        }
                    }
                    return response.IsSuccessStatusCode;
                }
                catch (Exception e)
                {
                    OnRetrievingDataFailed(e.Message);
                    return false;
                }
            });
        }

        protected async Task<T> PostDataObjectToAPI<T>(string apiUrl, T content, string accessToken)
        {
            return await Task.Run(async () =>
            {
                var client = SetAccessTokenHttpClient(accessToken);
                try
                {
                    var json = content == null ? null : SerializeObject(content);
                    var stringContent = content == null ? null : new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(apiUrl, stringContent);
                    //var contentNew = await response.Content.ReadAsStringAsync();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var reader = new System.IO.StreamReader(stream))
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        if (jsonReader == null)
                            return default(T);
                        if (response.IsSuccessStatusCode)
                        {
                            return _serializer.Deserialize<T>(jsonReader);
                        }
                        else
                        {
                            try
                            {
                                var error = _serializer.Deserialize<ServiceError>(jsonReader);
                                Crashes.TrackError(new Exception(response.ToString()), new Dictionary<string, string>{
                                                { "apiUrl", apiUrl },
                                                { "error", error?.ErrorDescription }
                                            });
                                OnRetrievingDataFailed(error.ErrorDescription);
                            }
                            catch (Exception e)
                            {
                                Crashes.TrackError(e, new Dictionary<string, string> { { "apiUrl", apiUrl } });
                                OnRetrievingDataFailed("Error occurred");
                            }
                        }
                    }
                    return default(T);
                }
                catch (Exception e)
                {
                    OnRetrievingDataFailed(e.Message);
                    return default(T);
                }
            });
        }

        protected async Task<bool> PutDataObjectToAPI<T>(string apiUrl, T content, string accessToken)
        {
            return await Task.Run(async () =>
            {
                var client = SetAccessTokenHttpClient(accessToken);
                try
                {
                    var json = SerializeObject(content);
                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PutAsync(apiUrl, stringContent);
                    if (!response.IsSuccessStatusCode)
                    {
                        try
                        {
                            using (var stream = await response.Content.ReadAsStreamAsync())
                            using (var reader = new System.IO.StreamReader(stream))
                            using (var jsonReader = new JsonTextReader(reader))
                            {
                                if (jsonReader == null)
                                    return false;
                                var error = _serializer.Deserialize<ServiceError>(jsonReader);
                                Crashes.TrackError(new Exception(response.ToString()), new Dictionary<string, string>{
                                                { "apiUrl", apiUrl },
                                                { "error", error?.ErrorDescription }
                                            });
                                OnRetrievingDataFailed(error.ErrorDescription);
                            }
                        }
                        catch (Exception e)
                        {
                            Crashes.TrackError(e, new Dictionary<string, string> { { "apiUrl", apiUrl } });
                            OnRetrievingDataFailed("Error occurred");
                        }
                    }
                    return response.IsSuccessStatusCode;
                }
                catch (Exception e)
                {
                    OnRetrievingDataFailed(e.Message);
                    return false;
                }
            });
        }

        void OnRetrievingDataFailed(string errorMessage)
        {
            RetrievingDataFailed?.Invoke(null, new RetrievingDataFailureEventArgs(errorMessage));
        }
    }
}
