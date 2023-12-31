﻿using DevExpress.Blazor;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DxBlazor.UI;
using DxChinook.Data;
using Microsoft.AspNetCore.Components;

namespace DxChinookWASM.Client.Services
{
    public class DevExtremeClientLoader : IDevExtremeLoader
    {
        readonly IServiceProvider serviceProvider;
        readonly NavigationManager navigationManager;
        public DevExtremeClientLoader(IServiceProvider serviceProvider, NavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            this.serviceProvider = serviceProvider;
        }
        public GridDevExtremeDataSource<TModel> GetDataSource<TKey, TModel>()
            where TKey : IEquatable<TKey>
            where TModel : class, new()
        {
            var store = serviceProvider.GetRequiredService<IDataStore<TKey, TModel>>() as ApiStore<TKey, TModel>;
            ArgumentNullException.ThrowIfNull(store);
            var dataSource = new GridDevExtremeDataSource<TModel>(store.Http, navigationManager.ToAbsoluteUri(store.ControllerBase));
            return dataSource;
        }

        public async Task<LoadResult> GetLookupDataSource<TKey, TModel>(DataSourceLoadOptionsBase options, CancellationToken cancellationToken)
            where TKey : IEquatable<TKey>
            where TModel : class, new()
        {
            var store = serviceProvider.GetRequiredService<IDataStore<TKey, TModel>>() as ApiStore<TKey, TModel>;
            ArgumentNullException.ThrowIfNull(store);

            string url = navigationManager.ToAbsoluteUri(store.ControllerBase).ToString();
            using var response = await store.Http.GetAsync(options.ConvertToGetRequestUri(url), cancellationToken);
            response.EnsureSuccessStatusCode();
            using var responseStream = await response.Content.ReadAsStreamAsync();
            return (await System.Text.Json.JsonSerializer.DeserializeAsync<LoadResult>(responseStream, cancellationToken: cancellationToken))!;

        }
    }
}
