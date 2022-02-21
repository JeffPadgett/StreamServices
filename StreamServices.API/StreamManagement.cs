using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StreamServices.Core;
using StreamServices.Core.Models;
using StreamServices.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace StreamServices.API
{
    public class StreamManagement : BaseFunction
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _client;

        public StreamManagement(IHttpClientFactory httpClientFactory, IMapper mapper) : base(httpClientFactory)
        {
            _mapper = mapper;
            _client = httpClientFactory.CreateClient("Twitch");
        }

        //http://localhost:7071/api/Subscribe?userName=brokenswordx
        //Function is meant to pass the userId in and subtype
        [FunctionName("Subscribe")]
        public async Task<IActionResult> Subscribe([HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = null)] HttpRequest req,
            [Table("Tokens", Connection = "JobsStorage")] CloudTable cloudTable,
            [Table("Tokens", "Twitch", "1", Connection = "JobsStorage")] AppAccessToken appAccessToken,
            ILogger log)
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            var user = JsonConvert.DeserializeObject<TwitchUser>(body);

            if (user == null || string.IsNullOrWhiteSpace(user.Action) || string.IsNullOrEmpty(user.Id)) return new BadRequestResult();

            appAccessToken = await VerifyAccessToken(cloudTable, appAccessToken, log);
            AddAuthHeaderToTwichClient(_client, appAccessToken.AccessToken);
                   
            log.LogInformation($"Subscribing {user.Login}");
            TwitchSubscriptionInitalPost subObject = new TwitchSubscriptionInitalPost(user.Id, user.Action);
            string subPayLoad = JsonConvert.SerializeObject(subObject);
            var postRequestContent = new StringContent(subPayLoad, Encoding.UTF8, "application/json");

            string responseBody;

            var responseMessage = await _client.PostAsync("eventsub/subscriptions", postRequestContent);

            if (!responseMessage.IsSuccessStatusCode)
            {
                responseBody = await responseMessage.Content.ReadAsStringAsync();
                log.Log(LogLevel.Error, $"Error response body {responseBody}");
            }
            else
            {
                log.LogInformation($"Subscribed to {user.Login}'s stream");
                return new OkObjectResult($"Notifications will now be sent when {user.Action} on stream {user.Login}");
            }
            return new BadRequestObjectResult(responseBody + $" When attempting to subscribe {user.Login}");
        }


        [FunctionName("GetSubscriptions")]
        public async Task<IActionResult> GetSubscriptions(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Table("Tokens", Connection = "JobsStorage")] CloudTable cloudTable,
            [Table("Tokens", "Twitch", "1", Connection = "JobsStorage")] AppAccessToken appAccessToken,
            ILogger log)
        {
            appAccessToken = await VerifyAccessToken(cloudTable, appAccessToken, log);
            AddAuthHeaderToTwichClient(_client, appAccessToken.AccessToken);
            var response = await _client.GetAsync("https://api.twitch.tv/helix/eventsub/subscriptions");
            response.EnsureSuccessStatusCode();
            SubscriptionList sublist = JsonConvert.DeserializeObject<SubscriptionList>(await response.Content.ReadAsStringAsync());
            List<SubscriptionDto> dtos = _mapper.Map<List<SubscriptionDto>>(sublist.Subscriptions);
            foreach (var d in dtos)
            {
                d.Name = await GetUserNameForChannelId(d.BroadcasterUserId, appAccessToken);
            }
            //dtos.ForEach(x => x.Name = _client.GetFromJsonAsync<TwitchUsers>($"users?id={x.BroadcasterUserId}").GetAwaiter().GetResult().Users[0].Login);

            return new OkObjectResult(JsonConvert.SerializeObject(dtos.OrderBy(x => x.Name)));
        }

        [FunctionName("DeleteEventSubsciption")]
        public async Task<IActionResult> DeleteEventSubsciption(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = null)] HttpRequest req,
            [Table("Tokens", Connection = "JobsStorage")] CloudTable cloudTable,
            [Table("Tokens", "Twitch", "1", Connection = "JobsStorage")] AppAccessToken appAccessToken,
            ILogger log)
        {
            appAccessToken = await VerifyAccessToken(cloudTable, appAccessToken, log);
            AddAuthHeaderToTwichClient(_client, appAccessToken.AccessToken);
            string id = req.Query["id"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return new BadRequestResult();
            }
            var response = await _client.DeleteAsync($"eventsub/subscriptions?id={id}");
            if (!response.IsSuccessStatusCode)
            {
                return new BadRequestObjectResult("No Event to unsubscribe from");
            }

            return new OkResult();
        }

        [FunctionName("CheckUserName")]
        public async Task<IActionResult> CheckUserName(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Table("Tokens", Connection = "JobsStorage")] CloudTable cloudTable,
            [Table("Tokens", "Twitch", "1", Connection = "JobsStorage")] AppAccessToken appAccessToken,
            ILogger log)
        {
            appAccessToken = await VerifyAccessToken(cloudTable, appAccessToken, log);

            AddAuthHeaderToTwichClient(_client, appAccessToken.AccessToken);

            string channelName = req.Query["name"];
            if (string.IsNullOrWhiteSpace(channelName))
            {
                return new BadRequestResult();
            }
            var users = await _client.GetFromJsonAsync<TwitchUsers>($"users?login={channelName}");

            if (users is null) return new BadRequestResult();

            var user = users.Users.FirstOrDefault();

            if (user is null) return new BadRequestResult();

            return new OkObjectResult(JsonConvert.SerializeObject(user));
        }


        private void AddAuthHeaderToTwichClient(HttpClient httpClient, string accessToken)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }
    }
}
