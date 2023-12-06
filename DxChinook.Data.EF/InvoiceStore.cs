using AutoMapper;
using DxChinook.Data.EF.Models;
using DxChinook.Data.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxChinook.Data.EF
{
    public class InvoiceStore : EFDataStore<ChinookContext, int, InvoiceModel, Invoice>
    {
        public InvoiceStore(ChinookContext context, IMapper mapper, IValidator<Invoice> validator) : base(context, mapper, validator)
        {

        }

        public override string KeyField => nameof(Invoice.InvoiceId);

        public override int ModelKey(InvoiceModel model) => model.InvoiceId;

        public override void SetModelKey(InvoiceModel model, int key) => model.InvoiceId = key;

        protected override int DBModelKey(Invoice model) => model.InvoiceId;
    }

    public class InvoiceValidator : AbstractValidator<Invoice>
    {
        public InvoiceValidator()
        {

        }
    }

    public class InvoiceLineStore : EFDataStore<ChinookContext, int, InvoiceLineModel, InvoiceLine>, IInvoiceLineStore
    {
        public InvoiceLineStore(ChinookContext context, IMapper mapper, IValidator<InvoiceLine> validator) : base(context, mapper, validator)
        {

        }

        public override string KeyField => nameof(InvoiceLine.InvoiceLineId);

        public async Task<List<InvoiceLineModel>> GetByInvoiceIdAsync(int invoiceId)
        {
            var result = await Query().Where(i => i.InvoiceId == invoiceId).ToListAsync();
            return result;
        }

        public override int ModelKey(InvoiceLineModel model) => model.InvoiceLineId;

        public override void SetModelKey(InvoiceLineModel model, int key) => model.InvoiceLineId = key;

        public async Task Store(int invoiceId, params InvoiceLineModel[] items)
        {
            foreach (var item in items) item.InvoiceId = invoiceId;

            var ids = items.Select(i => i.InvoiceLineId).ToList();
            var idsToDelete = await EFQuery().Where(i => i.InvoiceId == invoiceId && !ids.Contains(i.InvoiceLineId)).Select(i => i.InvoiceLineId).ToArrayAsync();
            await DeleteAsync(idsToDelete);
            await UpdateAsync(items.Where(i => i.InvoiceLineId > 0).ToArray());
            await CreateAsync(items.Where(i => i.InvoiceLineId == 0).ToArray());
        }

        protected override int DBModelKey(InvoiceLine model) => model.InvoiceLineId;


    }
    public class InvoiceLineValidator : AbstractValidator<InvoiceLine>
    {
        public InvoiceLineValidator()
        {

        }
    }

}
