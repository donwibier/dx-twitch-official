using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxChinook.Data.Models
{
    public class ArtistModel
    {
        public int ArtistId { get; set; }
        public string? Name { get; set; }
    }

    public class ArtistModelValidator: AbstractValidator<ArtistModel>
    {
        public ArtistModelValidator() {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
