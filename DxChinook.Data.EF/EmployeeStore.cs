using AutoMapper;
using DxChinook.Data.EF.Models;
using DxChinook.Data.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxChinook.Data.EF
{
    public class EmployeeStore : EFDataStore<ChinookContext, int, EmployeeModel, Employee>
    {
        public EmployeeStore(ChinookContext context, IMapper mapper, IValidator<Employee> validator) : base(context, mapper, validator)
        {

        }

        public override string KeyField =>nameof(EmployeeModel.EmployeeId);

        public override int ModelKey(EmployeeModel model) => model.EmployeeId;

        public override void SetModelKey(EmployeeModel model, int key) => model.EmployeeId = key;

        protected override int DBModelKey(Employee model) => model.EmployeeId;
    }

    public class EmployeeValidator: AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {

        }
    }
}
