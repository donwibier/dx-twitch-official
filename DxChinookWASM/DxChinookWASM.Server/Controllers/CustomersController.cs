using DevExtreme.AspNet.Mvc;
using DxChinook.Data;
using DxChinook.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DxChinookWASM.Server.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : BaseController<int, CustomerModel>
    {
        public CustomersController(IDataStore<int, CustomerModel> store) : base(store)
        {

        }
        [HttpGet]
        public override Task<IActionResult> Get(DataSourceLoadOptions loadOptions) => base.Get(loadOptions);

        [HttpGet("{key}")]
        public override IActionResult Get(int key) => base.Get(key);
        [HttpPost]
        public override Task<IActionResult> Post([FromBody]CustomerModel model) => base.Post(model);
        [HttpPut("{key}")]
        public override Task<IActionResult> Put(int key, [FromBody]CustomerModel model) => base.Put(key, model);
        [HttpDelete("{key}")]
        public override Task Delete(int key) => base.Delete(key);
    }
}
