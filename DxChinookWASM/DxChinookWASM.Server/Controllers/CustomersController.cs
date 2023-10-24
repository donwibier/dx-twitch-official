using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using DxChinook.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using DxChinook.Data.Models;

namespace DxChinookWASM.Server.Controllers
{  
    [Route("api/[controller]/[action]")]
    public class CustomersController : BaseController<int, CustomerModel>
    {
        public CustomersController(IDataStore<int, CustomerModel> store) : base(store)
        {

        }

        [HttpGet(Name ="Get")]
        public override async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            return await base.Get(loadOptions);
        }
        //[HttpGet]
        //public override IActionResult ByKey(int key)
        //{
        //    return base.Get(key);
        //}
        [HttpPost]
        public override async Task<IActionResult> Post(CustomerModel model)
        {
            return await base.Post(model);
        }

        [HttpPut]
        public override async Task<IActionResult> Put(CustomerModel model)
        {
            return await base.Put(model);
        }

        [HttpDelete]
        public override async Task Delete(int key) 
        {            
            await base.Delete(key);
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