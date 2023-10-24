using DevExpress.Blazor;
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
            //if (PaginateViaPrimaryKey)
            //{
            //    dataSource.CustomizeLoadOptions = (loadOptions) =>
            //    {
            //        // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            //        // This can make SQL execution plans more efficient.
            //        loadOptions.PrimaryKey = new[] { Store.KeyField };
            //        loadOptions.PaginateViaPrimaryKey = PaginateViaPrimaryKey;
            //    };
            //}
            return dataSource;
        }
    }
}
