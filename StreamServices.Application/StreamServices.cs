using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StreamServices.Core;
using StreamServices.Core.Models;
using StreamServices.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace StreamServices
{
    public class StreamServices 
    {

        [FunctionName("DiscordNotificationProcessor")]
        public async Task<IActionResult> DiscordNotificationProcessor([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            if (req.Headers["Twitch-Eventsub-Message-Type"] == "webhook_callback_verification")
            {
                return await CallbackVerification(req, log);
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            StreamStatusJson streamData = JsonConvert.DeserializeObject<StreamStatusJson>(requestBody);

            string discordMessage = SetDiscordMessage(streamData);
            string discordWebhook = SetChatRoom(streamData);

            var discordPayload = JsonConvert.SerializeObject(new DiscordChannelNotification(discordMessage));
            var discordPost = new StringContent(discordPayload, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(discordWebhook, discordPost);
            }

            return default;
        }

        private async Task<IActionResult> CallbackVerification(HttpRequest req, ILogger log)
        {
            var isAuthenticated = await VerifySignature(req);
            if (!string.IsNullOrEmpty(isAuthenticated))
            {
                log.LogInformation("User authenticated");
                return new OkObjectResult(isAuthenticated);
            }
            else
            {
                return new BadRequestResult();
            }
        }

        private string SetDiscordMessage(StreamStatusJson streamData)
        {
            if (streamData.Subscription.Type == "stream.offline")
            {
                return $"{streamData.Event.BroadcasterUserName} ended their stream :( " + "https://www.twitch.tv/" + streamData.Event.BroadcasterUserName;
            }
            else if (streamData.Subscription.Type == "stream.online")
            {
                return $"{streamData.Event.BroadcasterUserName} is now live! " + "https://www.twitch.tv/" + streamData.Event.BroadcasterUserName;
            }
            else if (streamData.Subscription.Type == "channel.follow")
            {
                return $"{streamData.Event.UserName} just followed {streamData.Event.BroadcasterUserName}s stream!";
            }
            else if (streamData.Subscription.Type == "channel.raid")
            {
                return $"{streamData.Event.FromBroadcasterUserName} just raided ${streamData.Event.ToBroadcasterUserName}. They now have ${streamData.Event.ViewerCount.ToString()} total viewers.";
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        private string SetChatRoom(StreamStatusJson streamData)
        {
            if (streamData.Subscription.Type == "stream.offline" || streamData.Subscription.Type == "stream.online")
            {
                return Environment.GetEnvironmentVariable("AnnouncementDiscordWebhook");
            }

            return Environment.GetEnvironmentVariable("LiveStreamDiscordWebhook");
        }

        private async Task<string> VerifySignature(HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var callbackJson = JsonConvert.DeserializeObject<ChallengeJson>(requestBody);
            var hmacMessage = req.Headers["Twitch-Eventsub-Message-Id"] + req.Headers["Twitch-Eventsub-Message-Timestamp"] + requestBody;

            var expectedSignature = "sha256=" + CreateHmacHash(hmacMessage, Environment.GetEnvironmentVariable("EventSubSecret"));

            var messageSignatureHeader = req.Headers["Twitch-Eventsub-Message-Signature"];
            if (expectedSignature == messageSignatureHeader)
            {
                return callbackJson.Challenge;
            }
            else
                return "";
        }

        private string CreateHmacHash(string data, string key)
        {
            var keybytes = UTF8Encoding.UTF8.GetBytes(key);
            var dataBytes = UTF8Encoding.UTF8.GetBytes(data);

            var hmac = new HMACSHA256(keybytes);
            var hmacBytes = hmac.ComputeHash(dataBytes);

            return BitConverter.ToString(hmacBytes).Replace("-", "").ToLower();
        }

    }


}
