using DevExpress.Blazor;
using DxBlazor.UI;
using DxChinook.Data;

namespace DxBlazorChinook.Data
{
    //transient
    public class DevExtremeLoader : IDevExtremeLoader
    {
        readonly IServiceProvider services;
        public DevExtremeLoader(IServiceProvider services) {
            this.services = services;
        }
        public GridDevExtremeDataSource<TModel> GetDataSource<TKey, TModel>()
            where TKey: IEquatable<TKey>
            where TModel : class, new()
        {
            var store = services.GetRequiredService<IDataStore<TKey, TModel>>();
            var d = new GridDevExtremeDataSource<TModel>(store.Query());
            

            return d;
        }
    }
}
