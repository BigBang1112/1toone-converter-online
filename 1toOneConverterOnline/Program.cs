using _1toOneConverterOnline;
using _1toOneConverterOnline.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Services;

GBX.NET.Gbx.LZO = new GBX.NET.LZO.Lzo();
GBX.NET.Gbx.ZLib = new GBX.NET.ZLib.ZLib();

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<ISettingsService, SettingsService>();
builder.Services.AddSingleton<IConversionService, ConversionService>();
builder.Services.AddScoped<LazyAssemblyLoader>();

await builder.Build().RunAsync();
