using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxChinook.Data.Models
{
    public class InvoiceModel
    {
        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? BillingAddress { get; set; }
        public string? BillingCity { get; set; }
        public string? BillingState { get; set; }
        public string? BillingCountry { get; set; }
        public string? BillingPostalCode { get; set; }
        public decimal Total { get; set; }

        public int CustomerId { get; set; }        
        public virtual string? CustomerName { get; set; }
        public CustomerModel Customer { get; set; }
        public List<InvoiceLineModel> InvoiceLines { get; set; } = new List<InvoiceLineModel>();
    }


    public class InvoiceModelValidator : AbstractValidator<InvoiceModel>
    {
        public InvoiceModelValidator() { }
    }

    public class InvoiceLineModel
    {
        public int InvoiceLineId { get; set; }
        public int InvoiceId { get; set; }
        public int TrackId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get => Quantity * UnitPrice; }
        //public virtual TrackModel Track { get; set; } = default!;
        public string TrackName { get; set; } = default!;
    }

    public class InvoiceLineModelValidator : AbstractValidator<InvoiceLineModel>
    {
        public InvoiceLineModelValidator() { }
    }

    public class TrackModel
    {
        public int TrackId { get; set; }
        public string Name { get; set; } = null!;
        public int? AlbumId { get; set; }
        public int MediaTypeId { get; set; }
        public int? GenreId { get; set; }
        public string? Composer { get; set; }
        public int Milliseconds { get; set; }
        public int? Bytes { get; set; }
        public decimal UnitPrice { get; set; }

    }
    public class TrackModelValidator : AbstractValidator<TrackModel>
    {
        public TrackModelValidator() { }
    }
}
