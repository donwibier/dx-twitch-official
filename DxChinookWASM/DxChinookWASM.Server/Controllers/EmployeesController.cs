using DxChinook.Data.Models;
using DxChinook.Data;
using Microsoft.AspNetCore.Mvc;
using DevExtreme.AspNet.Mvc;

namespace DxChinookWASM.Server.Controllers
{
        [Route("api/[controller]")]
        public class EmployeesController : BaseController<int, EmployeeModel>
        {
            public EmployeesController(IDataStore<int, EmployeeModel> store) : base(store)
            {

            }
            [HttpGet]
            public override Task<IActionResult> Get(DataSourceLoadOptions loadOptions) => base.Get(loadOptions);

            [HttpGet("{key}")]
            public override IActionResult Get(int key) => base.Get(key);
            [HttpPost]
            public override Task<IActionResult> Post([FromBody] EmployeeModel model) => base.Post(model);
            [HttpPut("{key}")]
            public override Task<IActionResult> Put(int key, [FromBody] EmployeeModel model) => base.Put(key, model);
            [HttpDelete("{key}")]
            public override Task Delete(int key) => base.Delete(key);
    }
}
