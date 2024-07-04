using Blazored.LocalStorage;
using MeshcapadeDemo.BlazorWASM;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://api-mspd-test.azurewebsites.net/") });
builder.Services.AddBlazorBootstrap();
builder.Services.AddBlazoredLocalStorage();
await builder.Build().RunAsync();
