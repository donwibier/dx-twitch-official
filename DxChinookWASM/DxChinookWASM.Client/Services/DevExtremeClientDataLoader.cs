using DevExpress.Blazor;
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
    }
}
