using DevExpress.Blazor;
using DxBlazor.UI;
using DxChinook.Data;
using Microsoft.AspNetCore.Components;

namespace DxChinookWASM.Client.Data
{
    public class DevExtremeClientLoader : IDevExtremeLoader
    {
        readonly IServiceProvider serviceProvider;

        public DevExtremeClientLoader(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public GridDevExtremeDataSource<TModel> GetDataSource<TKey, TModel>()
            where TKey : IEquatable<TKey>
            where TModel : class, new()
        {
            var store = serviceProvider.GetRequiredService<IDataStore<TKey, TModel>>() as ApiBaseStore<TKey, TModel>;
            ArgumentNullException.ThrowIfNull(store);                        
            var d = new GridDevExtremeDataSource<TModel>(store.HttpClient, store.GetApiUrl(store.ActionGet));
            return d;
        }
    }
}
