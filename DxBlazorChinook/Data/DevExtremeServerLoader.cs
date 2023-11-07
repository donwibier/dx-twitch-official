using DevExpress.Blazor;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DxBlazor.UI;
using DxChinook.Data;

namespace DxBlazorChinook.Data
{
    //transient
    public class DevExtremeServerLoader : IDevExtremeLoader
    {
        readonly IServiceProvider serviceProvider;
        public DevExtremeServerLoader(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public GridDevExtremeDataSource<TModel> GetDataSource<TKey, TModel>()
            where TKey : IEquatable<TKey>
            where TModel : class, new()
        {
            var store = serviceProvider.GetRequiredService<IDataStore<TKey, TModel>>() as IQueryableDataStore<TKey, TModel>;
            ArgumentNullException.ThrowIfNull(store);
            var dataSource = new GridDevExtremeDataSource<TModel>(store.Query());
            if (store.PaginateViaPrimaryKey)
            {
                dataSource.CustomizeLoadOptions = (loadOptions) =>
                {
                    // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
                    // This can make SQL execution plans more efficient.
                    loadOptions.PrimaryKey = new[] { store.KeyField };
                    loadOptions.PaginateViaPrimaryKey = store.PaginateViaPrimaryKey;
                };
            }
            return dataSource;
        }

        public async Task<LoadResult> GetLookupDataSource<TKey, TModel>(DataSourceLoadOptionsBase options, CancellationToken cancellationToken)
            where TKey : IEquatable<TKey>
            where TModel : class, new()
        {
            var store = serviceProvider.GetRequiredService<IDataStore<TKey, TModel>>() as IQueryableDataStore<TKey, TModel>;
            ArgumentNullException.ThrowIfNull(store);
            var dataSource = await DataSourceLoader.LoadAsync<TModel>(store.Query(), options, cancellationToken);
            return dataSource;
        }
    }
}
