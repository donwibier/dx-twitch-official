
using DxChinook.Data;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace DxChinookWASM.Client.Services
{
    public abstract class ApiStore<TKey, TModel> : IDataStore<TKey, TModel>
        where TKey : IEquatable<TKey>
        where TModel : class, new()

    {
        public HttpClient Http { get; }
        public ApiStore(HttpClient httpClient)
        {
            Http = httpClient;
        }

        public abstract string KeyField { get; }
        public abstract string ControllerBase { get; }
        public abstract TKey ModelKey(TModel model);
        public abstract void SetModelKey(TModel model, TKey key);

        public TModel GetByKey(TKey key)
        {
            var result = Http.GetFromJsonAsync<TModel>($"{ControllerBase}/{key}").GetAwaiter().GetResult();
            return result!;
        }

        public async Task<IDataResult> CreateAsync(params TModel[] items)
        {
            foreach (var item in items)
            {
                var result = await Http.PostAsJsonAsync(ControllerBase, item);
                var response = await result.Content.ReadAsStringAsync();
                if (result.IsSuccessStatusCode)
                {
                    var key = await result.Content.ReadFromJsonAsync<TKey>();
                    SetModelKey(item, key!);
                }
                else
                {
                    var err = JsonConvert.DeserializeObject<ValidationFailure[]>(response)!;
                    return new DataResult(DataMode.Create, nameof(TModel), new ValidationException(err));
                }
            }
            return new DataResult { Mode = DataMode.Create, Success = true };
        }

        public async Task<IDataResult> UpdateAsync(params TModel[] items)
        {
            foreach (var item in items)
            {
                var result = await Http.PutAsJsonAsync($"{ControllerBase}/{ModelKey(item)}", item);
                var response = await result.Content.ReadAsStringAsync();
                if (!result.IsSuccessStatusCode)
                {
                    var err = JsonConvert.DeserializeObject<ValidationFailure[]>(response)!;
                    return new DataResult(DataMode.Update, nameof(TModel), new ValidationException(err));
                }
            }
            return new DataResult { Mode = DataMode.Update, Success = true };
        }

        public async Task<IDataResult> DeleteAsync(params TKey[] ids)
        {
            foreach (var id in ids)
            {
                var result = await Http.DeleteAsync($"{ControllerBase}/{id}");
                var response = await result.Content.ReadAsStringAsync();
                if (!result.IsSuccessStatusCode)
                    return new DataResult(DataMode.Delete, nameof(TModel), new ValidationException(response));
            }

            return new DataResult { Mode = DataMode.Delete, Success = true };
        }
    }
}
