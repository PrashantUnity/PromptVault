using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PromptsValut;
using PromptsValut.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


// Register services
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IPromptService, PromptService>();
builder.Services.AddScoped<IPlaceholderParserService, PlaceholderParserService>();

await builder.Build().RunAsync();