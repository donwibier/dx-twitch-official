using DxBlazor.UI;
using DxBlazorChinook.Data;
using DxChinook.Data;
using DxChinook.Data.EF;
using DxChinook.Data.XPO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDevExpressBlazor(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
    options.SizeMode = DevExpress.Blazor.SizeMode.Medium;
});
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddTransient<IDevExtremeLoader, DevExtremeServerLoader>();
//builder.Services.RegisterDataServices(
//    builder.Configuration.GetConnectionString("ChinookConnection"));
builder.Services.RegisterXPODataServices(
	builder.Configuration.GetConnectionString("ChinookConnection"));

builder.Services.RegisterModelValidators();

builder.WebHost.UseWebRoot("wwwroot");
builder.WebHost.UseStaticWebAssets();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();