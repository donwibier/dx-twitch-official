using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;

namespace DxChinookWASM.Server
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

}
