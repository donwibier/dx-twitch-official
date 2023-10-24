using DxChinook.Data;
using DxChinook.Data.Models;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;
using DevExpress.Pdf.Native.BouncyCastle.Utilities;
using System.Text.Json;
using System.Text;
using DevExpress.Blazor.Base;

namespace DxChinookWASM.Client.Data
{

    public class ApiResult : IDataResult
    {
        public ApiResult() { }
        public ApiResult(DataMode mode, string propertyName, Exception err)
        {
            Mode = mode;
            Success = (err == null);
            if (!Success)
            {
                Exception = (err as ValidationException)!;
                if (Exception == null)
                    Exception = new ValidationException(new[] {
                        new ValidationFailure(propertyName, err!.InnerException == null ? err.Message : err.InnerException.Message)
                    });
            }
        }
        public bool Success { get; set; }
        public DataMode Mode { get; set; }
        public ValidationException Exception { get; set; } = default!;
    }

    public abstract class ApiBaseStore<TKey, TModel> : IDataStore<TKey, TModel>
        where TKey: IEquatable<TKey>
        where TModel: class, new()
    {
        public ApiBaseStore(HttpClient httpClient, NavigationManager navigationManager)
        {
            NavigationManager = navigationManager;
            HttpClient = httpClient;
            
        }

        public HttpClient HttpClient { get; }
        public NavigationManager NavigationManager { get; }

        public abstract string KeyField { get; }
        public abstract TKey ModelKey(TModel model);
        public abstract void SetModelKey(TModel model, TKey key);

        public abstract string ControllerBase { get; }
        public virtual string ActionByKey { get => "ByKey"; }
        public virtual string ActionGet { get => "Get"; }
        //public virtual string ActionPut { get => "Put"; }
        //public virtual string ActionPost { get => "Post"; }
        //public virtual string ActionDelete { get => "Delete"; }
        public virtual Uri GetApiUrl(string action)
        {
            var result = string.IsNullOrEmpty(action)
                ? NavigationManager.ToAbsoluteUri(ControllerBase)
                : NavigationManager.ToAbsoluteUri($"{ControllerBase}/{action}");
            return result;
        }
        public TModel GetByKey(TKey key)
        {
            var result = HttpClient.GetFromJsonAsync<TModel>(GetApiUrl($"{ActionByKey}/{key}")).GetAwaiter().GetResult();
            return result!;
        }

        public async Task<IDataResult> CreateAsync(params TModel[] items)
        {
            foreach(var item in items)
            {                
                var result = await HttpClient.PostAsJsonAsync(GetApiUrl(""), item);
                var response = await result.Content.ReadAsStringAsync();
                if (result.IsSuccessStatusCode)
                {
                    var key = await result.Content.ReadFromJsonAsync<TKey>();
                    SetModelKey(item, key);
                }
                else
                {
                    return new ApiResult(DataMode.Create, nameof(TModel), null!);
                }
            }

            return new ApiResult { Mode = DataMode.Create, Success = true };

        }
        public async Task<IDataResult> UpdateAsync(params TModel[] items)
        {
            foreach (var item in items)
            {
                var result = await HttpClient.PutAsJsonAsync<TModel>(GetApiUrl(""), item);
                var response = await result.Content.ReadAsStringAsync();
                if (result.IsSuccessStatusCode)
                {
                    var key = await result.Content.ReadFromJsonAsync<TKey>();
                    SetModelKey(item, key);
                }
                else
                {
                    return new ApiResult(DataMode.Create, nameof(TModel), null!);
                }
            }

            return new ApiResult { Mode = DataMode.Create, Success = true };
        }

        public async Task<IDataResult> DeleteAsync(params TKey[] ids)
        {
            foreach (var id in ids)
            {
                var result = await HttpClient.DeleteAsync(GetApiUrl($"{id}"));
                var response = await result.Content.ReadAsStringAsync();
                if (!result.IsSuccessStatusCode)
                {
                    return new ApiResult(DataMode.Delete, nameof(TModel), null!);
                }
            }
            return new ApiResult { Mode = DataMode.Delete, Success = true };
        }

        // these cannot be used on the client !!
        public IQueryable<T> Query<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TModel> Query()
        {
            throw new NotImplementedException();
        }
    }

    public class CustomerStore : ApiBaseStore<int, CustomerModel>
    {
        public CustomerStore(HttpClient httpClient, NavigationManager navigationManager) 
            : base(httpClient, navigationManager)
        {

        }
        public override string ControllerBase => "api/Customers";
        public override string KeyField => nameof(CustomerModel.CustomerId);
        public override int ModelKey(CustomerModel model) => model.CustomerId;
        public override void SetModelKey(CustomerModel model, int key) => model.CustomerId = key;
    }
}
