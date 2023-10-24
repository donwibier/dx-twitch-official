using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using DxChinook.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;



namespace DxChinookWASM.Server.Controllers
{
    //[Route("api/[controller]/[action]")]
    public class BaseController<TKey, TModel> : ControllerBase
        where TKey : IEquatable<TKey>
        where TModel : class, new()
    {
        

        public BaseController(IDataStore<TKey, TModel> store) {
            Store = store as IQueryableDataStore<TKey, TModel>;
            ArgumentNullException.ThrowIfNull(Store);
        }
        public IQueryableDataStore<TKey, TModel> Store { get; }
        protected bool PaginateViaPrimaryKey { get => false; }
        //[HttpGet]
        public virtual async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            loadOptions.PrimaryKey = new[] { Store.KeyField };
            loadOptions.PaginateViaPrimaryKey = PaginateViaPrimaryKey;

            return Ok(await DataSourceLoader.LoadAsync(Store.Query(), loadOptions));
        }

        public virtual IActionResult Get(TKey key)
        {
            return Ok(Store.GetByKey(key));
        }

        //[HttpPost]
        public virtual async Task<IActionResult> Post(TModel model) {

            var result = await Store.CreateAsync(model);

            if (result.Success)
                return Ok(Store.ModelKey(model));
            else
                return BadRequest(JsonConvert.SerializeObject(result.Exception.Errors));
        }

        //[HttpPut]
        public virtual async Task<IActionResult> Put(TKey key, TModel model) {
            var result = await Store.UpdateAsync(model);
            if (result.Success)
                return Ok(Store.ModelKey(model));
            else
                return BadRequest(JsonConvert.SerializeObject(result.Exception.Errors));
        }

        [HttpDelete]
        public virtual async Task Delete(TKey key) 
        {
            var result = await Store.DeleteAsync(key);
            if (!result.Success)
                throw result.Exception;
        }


        //[HttpGet]
        //public async Task<IActionResult> EmployeesLookup(DataSourceLoadOptions loadOptions) {
        //    var lookup = from i in _context.Employees
        //                 orderby i.Title
        //                 select new {
        //                     Value = i.EmployeeId,
        //                     Text = i.Title
        //                 };
        //    return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        //}

        
    }
}