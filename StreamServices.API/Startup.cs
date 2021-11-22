using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using StreamServices;
using StreamServices.API;
using StreamServices.Core;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace StreamServices.API
{
    public sealed class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient("Twitch",option =>
            {
                option.BaseAddress = new Uri(Environment.GetEnvironmentVariable(("BaseTwitchUrl")));
                option.DefaultRequestHeaders.Add("Accept", "application/json");
                option.DefaultRequestHeaders.Add("Client-ID", Environment.GetEnvironmentVariable("TwitchClientId"));


            ;});

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        }
    }

}

//if (clientId == "") clientId = Environment.GetEnvironmentVariable("ClientId");

//var client = HttpClientFactory.CreateClient();
//client.BaseAddress = new Uri(baseAddress);

//if (includeJson)
//{
//    client.DefaultRequestHeaders.Add("Accept", @"application/json");
//}
//if (!discordPost)
//{
//    client.DefaultRequestHeaders.Add("Client-ID", clientId);
//}

//return client;