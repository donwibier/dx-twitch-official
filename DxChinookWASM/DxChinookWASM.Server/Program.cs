//using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;
using DxChinook.Data.EF;
using DxChinook.Data;
using DxChinook.Data.Models;
using System.Xml.Linq;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using DxChinookWASM.Server;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(c => {
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChinookAPI", Version = "v1" });
//});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDevExpressBlazor(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
    options.SizeMode = DevExpress.Blazor.SizeMode.Medium;
});
builder.WebHost.UseStaticWebAssets();

builder.Services.RegisterDataServices(
    builder.Configuration.GetConnectionString("ChinookConnection")!);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseWebAssemblyDebugging();
} else {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseSwagger();
// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChinookAPI");
});

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.MapGet("/api/cust/{key}", (int key, IDataStore<int, CustomerModel> store) =>
    store.GetByKey(key)
        is CustomerModel model
            ? Results.Ok(model)
            : Results.NotFound()
).WithName("GetCustomer by ID").WithOpenApi();

app.MapGet("/api/cust", async (MinimalDataSourceLoadOptions loadOptions, IDataStore<int, CustomerModel> store) =>
{
    var s = store as IQueryableDataStore<int, CustomerModel>;
    if (s == null)
        return Results.BadRequest("Store doesn't implement IQueryableDataStore");

    loadOptions.PrimaryKey = new[] { store.KeyField };
    loadOptions.PaginateViaPrimaryKey = true;

    return Results.Ok(await DataSourceLoader.LoadAsync(s.Query(), loadOptions));
}).WithName("GetCustomers").WithOpenApi();

app.MapPost("/api/cust", async ([ValidateNever] CustomerModel model, IDataStore<int, CustomerModel> store) =>
{
    var result = await store.CreateAsync(model);
    return (result.Success)
        ? Results.Ok(store.ModelKey(model))
        : Results.BadRequest(JsonConvert.SerializeObject(result.Exception.Errors));
})
    
    .WithName("PostCustomer")
    .WithOpenApi();

app.MapPut("/api/cust/{key}", async (int key, [ValidateNever] CustomerModel model, IDataStore<int, CustomerModel> store) =>
{
    var result = await store.UpdateAsync(model);

    return (result.Success)
        ? Results.Ok(store.ModelKey(model))
        : Results.BadRequest(JsonConvert.SerializeObject(result.Exception.Errors));
}).WithName("PutCustomer").WithOpenApi();

app.MapDelete("/api/cust/{key}", async (int key, IDataStore<int, CustomerModel> store) =>
{
    var result = await store.DeleteAsync(key);
    if (!result.Success)
        throw result.Exception;
}).WithName("DeleteCustomer").WithOpenApi();



app.Run();