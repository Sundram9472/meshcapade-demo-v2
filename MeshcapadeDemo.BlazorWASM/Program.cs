using MeshcapadeDemo.BlazorWASM;
using MeshcapadeDemo.BlazorWASM.Modal;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://182.75.88.147:8159/") });
builder.Services.AddBlazorBootstrap();
builder.Services.AddSingleton<GlobalVariablesService>();
await builder.Build().RunAsync();
