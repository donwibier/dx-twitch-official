using AutoMapper;
using AutoMapper.Internal;
using DxChinook.Data.Models;
using DxChinook.Data.XPO.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DxChinook.Data.XPO
{
	public static class RegisterServices
	{
		public static IServiceCollection RegisterXPODataServices(this IServiceCollection services, string connectionString)

		{
			services.AddAutoMapper(cfg => cfg.AddProfile<ChinookXPOMappingProfile>());
			services.AddChinookXpoDefaultDataLayer(ServiceLifetime.Singleton, options =>
			{
				options.UseConnectionString(connectionString);
				options.UseAutoCreationOption(DevExpress.Xpo.DB.AutoCreateOption.None);
				options.UseEntityTypes(ConnectionHelper.GetPersistentTypes());
			});
			//services.AddDbContext<ChinookContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);

			//more services here...
			services.AddScoped<IDataStore<int, CustomerModel>, CustomerStore>();
			services.AddScoped<IValidator<XPCustomer>, XPCustomerValidator>();

			services.AddScoped<IDataStore<int, ArtistModel>, ArtistStore>();
			services.AddScoped<IValidator<XPArtist>, XPArtistValidator>();

			services.AddScoped<IDataStore<int, EmployeeModel>, EmployeeStore>();
			services.AddScoped<IValidator<XPEmployee>, XPEmployeeValidator>();

			return services;
		}

	}

	public class ChinookXPOMappingProfile : Profile
	{
		public ChinookXPOMappingProfile()
		{
            //Internal().RecursiveQueriesMaxDepth = 1;
			
            CreateMap<XPCustomer, CustomerModel>()
				.ForMember(dest=>dest.SupportRepId, opt=>opt.MapFrom(src=>src.SupportRep.EmployeeId))
				.ForMember(dest => dest.SupportRepName, opt => opt.MapFrom(src => src.SupportRep != null ? $"{src.SupportRep.FirstName} {src.SupportRep.LastName}" : ""))
				.ReverseMap()
					.ForMember(dest => dest.SupportRepID, opt => opt.Ignore())
					.ForMember(dest => dest.SupportRep, opt => opt.MapFrom((src, dest) => dest.Session.GetObjectByKey<XPEmployee>(src.SupportRepId)));
			CreateMap<XPArtist, ArtistModel>()
				.ReverseMap();

			CreateMap<XPEmployee, EmployeeModel>()
				.ForMember(dest => dest.ReportsTo, opt => opt.MapFrom(src=>src.ReportsTo != null ? src.ReportsTo.EmployeeId : 0))
				.ReverseMap()
					.ForMember(dest => dest.ReportsTo, opt => opt.MapFrom((src, dest) => dest.Session.GetObjectByKey<XPEmployee>(src.ReportsTo)));
					

		}
	}

}
