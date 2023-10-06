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
    public class CustomerStore : EFDataStore<ChinookContext, int, CustomerModel, Customer>
    {
        public CustomerStore(ChinookContext context, IMapper mapper, 
            IValidator<Customer> validator) : base(context, mapper, validator)
        {

        }

        public override string KeyField => nameof(Customer.CustomerId);
        public override int ModelKey(CustomerModel model) => model.CustomerId;
        public override void SetModelKey(CustomerModel model, int key) => model.CustomerId = key;
        protected override int DBModelKey(Customer model) => model.CustomerId;
    }

    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.LastName)
                .NotEmpty();
           
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .CustomAsync(async (email, ctx, ct) => {
                    if (ctx.RootContextData.ContainsKey(CustomerStore.CtxMode) &&
                        ctx.RootContextData.ContainsKey(CustomerStore.CtxStore))
                    {
                        var store = (CustomerStore)ctx.RootContextData[CustomerStore.CtxStore];
                        var mode = (DataMode)ctx.RootContextData[CustomerStore.CtxMode];
                        var cust = ctx.InstanceToValidate;

                        if (await store.Query().Where(c => c.Email == email && c.CustomerId != cust.CustomerId).AnyAsync(ct))
                        {
                            ctx.AddFailure("Email address is already in use");
                        }
                    }
                });
        }
    }

}
