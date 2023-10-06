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
    public class ArtistStore : EFDataStore<ChinookContext, int, ArtistModel, Artist>
    {
        public ArtistStore(ChinookContext context, IMapper mapper, IValidator<Artist> validator) 
            : base(context, mapper, validator)
        {

        }

        public override string KeyField => nameof(Artist.ArtistId);
        public override int ModelKey(ArtistModel model) => model.ArtistId;
        public override void SetModelKey(ArtistModel model, int key) => model.ArtistId = key;
        protected override int DBModelKey(Artist model) => model.ArtistId;
    }

    public class ArtistValidator : AbstractValidator<Artist>
    {
        public ArtistValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
