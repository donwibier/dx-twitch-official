﻿using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;
using DxChinook.Data.Models;
using DxChinook.Data;
using Microsoft.AspNetCore.Mvc;


namespace DxChinookv8
{
    public class MinimalDataSourceLoadOptions : DataSourceLoadOptions
    {
        public static ValueTask<MinimalDataSourceLoadOptions> BindAsync(HttpContext httpContext)
        {
            var loadOptions = new MinimalDataSourceLoadOptions();
            DataSourceLoadOptionsParser.Parse(loadOptions, key => httpContext.Request.Query[key]);

            return ValueTask.FromResult(loadOptions);
        }
    }

    public static class MapDataStoreControllerExtension
    {
        public static WebApplication MapStoreController<TKey, TModel>(this WebApplication app, string route, bool mapCreate = true, bool mapUpdate = true, bool mapDelete = true)
            where TKey: IEquatable<TKey>
            where TModel: class, new()
        {
            app.MapGet($"api/{route}", async (MinimalDataSourceLoadOptions loadOptions, [FromServices] IDataStore<TKey, TModel> store) =>
            {
                var s = store as IQueryableDataStore<TKey, TModel>;
                if (s == null)
                    return Results.BadRequest("Store doesn't implement IQueryableDataStore");

                loadOptions.PrimaryKey = new[] { store.KeyField };
                loadOptions.PaginateViaPrimaryKey = true;

                return Results.Ok(await DataSourceLoader.LoadAsync(s.Query(), loadOptions));
            }).WithName($"Get{route}").WithOpenApi();

            //get by key
            app.MapGet($"api/{route}/{{key}}", (TKey key, [FromServices] IDataStore<TKey, TModel> store) =>
                store.GetByKey(key)
                    is TModel model
                        ? Results.Ok(model)
                        : Results.NotFound()
            ).WithName($"Get{route} by ID").WithOpenApi();

            if (mapCreate)
            {
                app.MapPost($"/api/{route}", async ([ValidateNever, FromBody] TModel[] models, [FromServices] IDataStore<TKey, TModel> store) =>
                {
                    var result = await store.CreateAsync(models);
                    return (result.Success)
                        ? Results.Ok(models)
                        : Results.BadRequest(JsonConvert.SerializeObject(result.Exception.Errors));
                }).WithName($"Post{route}").WithOpenApi();
            }

            if (mapUpdate)
            {
                app.MapPut($"/api/{route}", async ([ValidateNever, FromBody] TModel[] models, [FromServices] IDataStore<TKey, TModel> store) =>
                {
                    var result = await store.UpdateAsync(models);

                    return (result.Success)
                        ? Results.Ok(models)
                        : Results.BadRequest(JsonConvert.SerializeObject(result.Exception.Errors));
                }).WithName($"Put{route}").WithOpenApi();
            }

            if (mapDelete)
            {
                app.MapDelete($"/api/{route}/{{key}}", async (TKey key, [FromServices] IDataStore<TKey, TModel> store) =>
                {
                    var result = await store.DeleteAsync(key);
                    if (!result.Success)
                        throw result.Exception;
                }).WithName($"Delete{route}").WithOpenApi();
            }
            return app;            
        }
    }
}
