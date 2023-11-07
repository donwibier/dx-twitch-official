using DevExpress.Blazor;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxBlazor.UI
{
    public interface IDevExtremeLoader
    {
        Task<LoadResult> GetLookupDataSource<TKey, TModel>(DataSourceLoadOptionsBase options, CancellationToken cancellationToken)
            where TKey : IEquatable<TKey>
            where TModel : class, new();
        GridDevExtremeDataSource<TModel> GetDataSource<TKey, TModel>()
            where TKey : IEquatable<TKey>
            where TModel : class, new();
    }



}
