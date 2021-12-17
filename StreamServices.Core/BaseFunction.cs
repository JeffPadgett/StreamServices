using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamServices.Dto;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StreamServices.Core
{
    public abstract class BaseFunction
    {
        protected IHttpClientFactory HttpClientFactory { get; }
        protected BaseFunction(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }

        public HttpClient GetHttpClient(string baseAddress, string clientId = "", bool includeJson = true, bool discordPost = false)
        {
            if (clientId == "")
            {
                clientId = Environment.GetEnvironmentVariable("TwitchClientId");
            }

            var client = HttpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseAddress);

            if (includeJson)
            {
                client.DefaultRequestHeaders.Add("Accept", @"application/json");
            }
            if (!discordPost)
            {
                client.DefaultRequestHeaders.Add("Client-ID", clientId);
            }

            return client;
        }

        protected async Task GetAccessToken(AppAccessToken accessToken)
        {
            var clientId = Environment.GetEnvironmentVariable("TwitchClientId");
            var clientSecret = Environment.GetEnvironmentVariable("TwitchClientSecret");

            using var client = GetHttpClient("https://id.twitch.tv");
            var result = await client.PostAsync($"/oauth2/token?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials&scope=", new StringContent(""));

            result.EnsureSuccessStatusCode();

            var refreshedToken = JsonConvert.DeserializeObject<AppAccessToken>(await result.Content.ReadAsStringAsync());
            accessToken.AccessToken = refreshedToken.AccessToken;
            accessToken.ExpiresAtUTC = DateTime.UtcNow.AddSeconds(refreshedToken.ExpiresInSeconds);
        }

        protected async Task<string> GetUserNameForChannelId(string channelId, AppAccessToken accessToken)
        {
            var client = GetHttpClient("https://api.twitch.tv/helix/");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken.AccessToken}");

            var body = await client.GetAsync($"users?id={channelId}");
            //.ContinueWith(msg => msg.Result.Content.ReadAsStringAsync()).Result;
            //var obj = JObject.Parse(body);
            var result = JsonConvert.DeserializeObject<TwitchUsers>(await body.Content.ReadAsStringAsync());
            return result.Users.FirstOrDefault().Login;
            //return obj["data"][0]["login"].ToString();
        }

        public async Task<string> GetChannelIdForUserName(string userName, AppAccessToken accessToken)
        {
            var client = GetHttpClient("https://api.twitch.tv/helix/");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken.AccessToken}");

            string body;
            try
            {
                var msg = await client.GetAsync($"users?login={userName}");
                msg.EnsureSuccessStatusCode();
                body = await msg.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                throw e;
            }
            catch
            {
                if (await GetUserNameForChannelId(userName, accessToken) != string.Empty)
                    return userName;
                else
                    return string.Empty;
            }

            var obj = JObject.Parse(body);
            return obj["data"][0]["id"].ToString();
        }

        protected async Task<string> IdentifyUser(string user, AppAccessToken accessToken)
        {
            if (char.IsDigit(user[0]))
                return await GetUserNameForChannelId(user, accessToken);
            else
                return user;
        }



        protected async Task<AppAccessToken> VerifyAccessToken(CloudTable cloudTable, AppAccessToken appAccessToken, ILogger log)
        {
            log.LogInformation("Verifying Access Token");

            if (appAccessToken is null)
            {
                appAccessToken = new AppAccessToken()
                {
                    AccessToken = "123",
                    ExpiresAtUTC = DateTime.UtcNow.AddMinutes(-15),
                    PartitionKey = "Twitch",
                    RowKey = "1"
                };
                TableOperation tableOperation = TableOperation.InsertOrReplace(appAccessToken);
            }

            if (appAccessToken.ExpiresAtUTC < DateTime.UtcNow.AddSeconds(-30))
            {
                await GetAccessToken(appAccessToken);
                TableOperation tableOperation = TableOperation.InsertOrReplace(appAccessToken);
                await cloudTable.ExecuteAsync(tableOperation);
            }

            return appAccessToken;
        }
    }
}
