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
	public class EmployeeStore: XPDataStore<int, EmployeeModel, XPEmployee>
	{
		public EmployeeStore(IDataLayer dataLayer, IMapper mapper, IValidator<XPEmployee> validator) 
			: base(dataLayer, mapper, validator)
		{

		}
		public override string KeyField => nameof(XPEmployee.EmployeeId);
		public override int ModelKey(EmployeeModel model) => model.EmployeeId;
		public override void SetModelKey(EmployeeModel model, int key) => model.EmployeeId = key;
		protected override int DBModelKey(XPEmployee model) => model.EmployeeId;

		public override IQueryable<EmployeeModel> Query()
		{
			var result = base.Query();
			return result;
		}
	}
	public class XPEmployeeValidator : AbstractValidator<XPEmployee>
	{
		public XPEmployeeValidator()
		{
      //      RuleFor(x => x.EmployeeId)                                
      //          .CustomAsync(async (id, ctx, ct) => {
      //              if (ctx.RootContextData.ContainsKey(CustomerStore.CtxMode) &&
      //                  ctx.RootContextData.ContainsKey(CustomerStore.CtxStore))
      //              {
      //                  var store = (EmployeeStore)ctx.RootContextData[CustomerStore.CtxStore];
      //                  var mode = (DataMode)ctx.RootContextData[CustomerStore.CtxMode];
      //                  var emp = ctx.InstanceToValidate;

						//if (mode == DataMode.Delete && (emp.Customers.Count() > 0 || emp.XPEmployeeCollection.Count() > 0) )
      //                      ctx.AddFailure("This Employee can not be deleted because it is assigned to customers or it has employees reporting");
      //              }
      //          });
        }
	}
}
