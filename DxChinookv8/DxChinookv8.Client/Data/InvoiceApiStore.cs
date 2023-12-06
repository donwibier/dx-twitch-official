using DevExpress.Blazor.Base;
using DxChinook.Data;
using DxChinook.Data.Models;
using DxChinookWASM.Client.Services;
using System.Net.Http.Json;

namespace DxChinookv8.Client.Data
{
    public class InvoiceApiStore : ApiStore<int, InvoiceModel>
    {
        public InvoiceApiStore(HttpClient httpClient) : base(httpClient)
        {

        }

        public override string KeyField => nameof(InvoiceModel.InvoiceId);

        public override string ControllerBase => "api/Invoices";

        public override int ModelKey(InvoiceModel model) => model.InvoiceId;

        public override void SetModelKey(InvoiceModel model, int key)=>model.InvoiceId = key;
    }

    public class InvoiceLineApiStore : ApiStore<int, InvoiceLineModel>, IInvoiceLineStore
    {
        public InvoiceLineApiStore(HttpClient httpClient) : base(httpClient)
        {

        }

        public override string KeyField => nameof(InvoiceLineModel.InvoiceLineId);

        public override string ControllerBase => "api/InvoiceLines";

        public async Task<List<InvoiceLineModel>> GetByInvoiceIdAsync(int invoiceId)
        {
           var result = await Http.GetFromJsonAsync<List<InvoiceLineModel>>($"{ControllerBase}/ByInvoice/{invoiceId}");
           return result!;
        }

        public override int ModelKey(InvoiceLineModel model) => model.InvoiceLineId;        

        public override void SetModelKey(InvoiceLineModel model, int key) => model.InvoiceLineId = key;

        public async Task Store(int invoiceId, params InvoiceLineModel[] items)
        {
            var result = await Http.PutAsJsonAsync($"{ControllerBase}/ByInvoice/{invoiceId}", items);
            //return result!;

            //foreach (var item in items) item.InvoiceId = invoiceId;

            //var ids = items.Select(i => i.InvoiceLineId).ToList();
            //var idsToDelete = await EFQuery().Where(i => i.InvoiceId == invoiceId && !ids.Contains(i.InvoiceLineId)).Select(i => i.InvoiceLineId).ToArrayAsync();
            //await DeleteAsync(idsToDelete);
            //await UpdateAsync(items.Where(i => i.InvoiceLineId > 0).ToArray());
            //await CreateAsync(items.Where(i => i.InvoiceLineId == 0).ToArray());

        }
    }
}
