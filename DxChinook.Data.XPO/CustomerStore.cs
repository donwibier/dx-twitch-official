using AutoMapper;
using DevExpress.Xpo;
using DxChinook.Data.Models;
using DxChinook.Data.XPO.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxChinook.Data.XPO
{
	public class CustomerStore : XPDataStore<int, CustomerModel, XPCustomer>
	{
		public CustomerStore(IDataLayer dataLayer, IMapper mapper, IValidator<XPCustomer> validator) 
			: base(dataLayer, mapper, validator)
		{

		}
		public override string KeyField => nameof(XPCustomer.CustomerId);

		public override int ModelKey(CustomerModel model) => model.CustomerId;
		public override void SetModelKey(CustomerModel model, int key) => model.CustomerId = key;
		protected override int DBModelKey(XPCustomer model) => model.CustomerId;
	}

	public class XPCustomerValidator : AbstractValidator<XPCustomer>
	{
		public XPCustomerValidator()
		{
			RuleFor(x => x.LastName)
				.NotEmpty();
			RuleFor(x => x.SupportRepID)
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
