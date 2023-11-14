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
	public class ArtistStore : XPDataStore<int, ArtistModel, XPArtist>
	{
		public ArtistStore(IDataLayer dataLayer, IMapper mapper, IValidator<XPArtist> validator) 
			: base(dataLayer, mapper, validator)
		{

		}
		public override string KeyField => nameof(XPArtist.ArtistId);
		public override int ModelKey(ArtistModel model) => model.ArtistId;
		public override void SetModelKey(ArtistModel model, int key) => model.ArtistId = key;
		protected override int DBModelKey(XPArtist model) => model.ArtistId;
	}

	public class XPArtistValidator : AbstractValidator<XPArtist>
	{
		public XPArtistValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty();
		}
	}
}
