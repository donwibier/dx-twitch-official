using AutoMapper;
using DxChinook.Data.EF.Models;
using DxChinook.Data.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DxChinook.Data.EF
{
    public static class RegisterServices
    {
        public static IServiceCollection RegisterDataServices(this IServiceCollection services, string connectionString)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile<ChinookMappingProfile>());
            services.AddDbContext<ChinookContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
            
            //more services here...
            services.AddScoped<IDataStore<int, CustomerModel>, CustomerStore>();
            services.AddScoped<IValidator<Customer>, CustomerValidator>();
            services.AddScoped<IDataStore<int, ArtistModel>, ArtistStore>();
            services.AddScoped<IValidator<Artist>, ArtistValidator>();

            return services;
        }
    }

    public class ChinookMappingProfile : Profile
    {
        public ChinookMappingProfile()
        {
            CreateMap<Customer, CustomerModel>()
                .ReverseMap();
            CreateMap<Artist, ArtistModel>()
                .ReverseMap();
        }
    }
}