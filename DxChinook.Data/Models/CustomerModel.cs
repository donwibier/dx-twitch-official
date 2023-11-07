using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxChinook.Data.Models
{
    
    public class CustomerModel
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Company { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string Email { get; set; } = null!;
        
        public int SupportRepId { get; set; }
        public string SupportRepName { get; set; }

        public string FullName { get => $"{LastName}, {FirstName}"; }
    }
    public class CustomerModelValidator : AbstractValidator<CustomerModel>
    {
        public CustomerModelValidator()
        {
            RuleFor(x => x.SupportRepId)
                .NotEmpty();
            //RuleFor(x => x.FirstName)
            //    .NotEqual("Don");
            //RuleFor(x => x.Email)
            //    .NotEmpty()
            //    .EmailAddress();
        }
    }
}
