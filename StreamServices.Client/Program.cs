using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StreamServices.Client;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();

string baseUri = builder.HostEnvironment.BaseAddress;
#if DEBUG
baseUri = "http://localhost:7071";
#endif

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseUri) });

builder.Services.AddMsalAuthentication(options =>//Wires up Azure Active Directory. 
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
});

await builder.Build().RunAsync();
