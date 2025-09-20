using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PromptsValut;
using PromptsValut.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


// Register services
builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IPromptService, PromptService>();
builder.Services.AddScoped<IPlaceholderParserService, PlaceholderParserService>();
builder.Services.AddHttpClient();

await builder.Build().RunAsync();