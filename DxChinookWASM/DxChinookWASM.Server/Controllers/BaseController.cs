using DevExpress.Xpo.DB;
using DevExpress.XtraPrinting.Native;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using DxChinook.Data;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Collections;

namespace DxChinookWASM.Server.Controllers
{
    

    public abstract class BaseController<TKey, TModel> : Controller
        where TKey: IEquatable<TKey>
        where TModel : class, new()
    {
        public BaseController(IDataStore<TKey, TModel> store)
            : base() 
        {
            Store = store;
        }

        public IDataStore<TKey, TModel> Store { get; }

        protected virtual bool PrimaryKeyPagination { get => false; }
        protected virtual string PrimaryKey { get; } = string.Empty;

        public async virtual Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new string[] { PrimaryKey };
            loadOptions.PaginateViaPrimaryKey = PrimaryKeyPagination;

            return Json(await DataSourceLoader.LoadAsync(Store.Query(), loadOptions));
        }

        public virtual IActionResult Get(TKey key)
        {
            var result = Store.GetByKey(key);
            return Ok(result);
        }

        public virtual async Task<IActionResult> Post(TModel model)
        {
            //var model = new TModel();
            //PopulateModel(model, values);

            if (!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var r = await Store.CreateAsync(model);

            if (r.Success)
                return Ok(Store.ModelKey(model));
            else
                return BadRequest(r.Exception);
        }

        public async virtual Task<IActionResult> Put(TModel model)
        {
            //var model = Store.GetByKey(key);
            //PopulateModel(model, values);

            if (!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var r = await Store.UpdateAsync(model);

            if (r.Success)
                return Ok(Store.ModelKey(model));
            else
                return BadRequest(r.Exception);
        }

        public async virtual Task Delete(TKey key)
        {
            var result = await Store.DeleteAsync(key);
            if (!result.Success)
                throw result.Exception;
        }

        protected string GetFullErrorMessage(ModelStateDictionary modelState)
        {
            var messages = new List<string>();

            foreach (var entry in modelState)
            {
                foreach (var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }
            return string.Join(" ", messages);
        }
    }
}
