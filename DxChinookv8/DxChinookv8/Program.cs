using DxBlazor.UI;

using DxChinook.Data;
using DxChinook.Data.EF;
using DxChinookv8.Client.Pages;

using DxChinookv8.Data;
using DxChinookv8.Components;
using DxChinook.Data.Models;
using DxChinookv8;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDevExpressBlazor(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
    options.SizeMode = DevExpress.Blazor.SizeMode.Medium;
});
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddTransient<IDevExtremeLoader, DevExtremeServerLoader>();
builder.Services.RegisterDataServices(
    builder.Configuration.GetConnectionString("ChinookConnection"));
builder.Services.RegisterModelValidators();

var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment()) {
    app.UseWebAssemblyDebugging();
} else {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseSwagger();
// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChinookAPI");
});
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.MapStoreController<int, ArtistModel>("Artists");
app.MapStoreController<int, CustomerModel>("Customers");
app.MapStoreController<int, EmployeeModel>("Employees");
app.MapStoreController<int, InvoiceModel>("Invoices");
app.MapStoreController<int, InvoiceLineModel>("InvoiceLines");
app.MapGet($"api/InvoiceLines/ByInvoice/{{invoiceId}}", async (int invoiceId, IInvoiceLineStore store) =>
    await store.GetByInvoiceIdAsync(invoiceId)
                    is List<InvoiceLineModel> result
                        ? Results.Ok(result)
                        : Results.NotFound()
).WithName($"GetInvoiceItemsById").WithOpenApi();

app.MapPut($"api/InvoiceLines/ByInvoice/{{invoiceId}}", 
    async (int invoiceId, [FromBody] InvoiceLineModel[] invoiceLines, [FromServices] IInvoiceLineStore store) =>
    {
        await store.Store(invoiceId, invoiceLines);
    }).WithName($"PutInvoiceItems").WithOpenApi();

app.Run();