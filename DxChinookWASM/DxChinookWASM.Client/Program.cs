using DxBlazor.UI;
using DxChinook.Data;
using DxChinook.Data.Models;
using DxChinookWASM.Client;
using DxChinookWASM.Client.Data;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddDevExpressBlazor(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
    options.SizeMode = DevExpress.Blazor.SizeMode.Medium;
});
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddTransient<IDevExtremeLoader, DevExtremeClientLoader>();
builder.Services.AddScoped<IDataStore<int, CustomerModel>, CustomerStore>();

await builder.Build().RunAsync();