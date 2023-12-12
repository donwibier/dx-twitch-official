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
            
            services.AddScoped<IDataStore<int, EmployeeModel>, EmployeeStore>();
            services.AddScoped<IValidator<Employee>, EmployeeValidator>();

            services.AddScoped<IDataStore<int, InvoiceModel>, InvoiceStore>();
            services.AddScoped<IValidator<Invoice>, InvoiceValidator>();
            
            services.AddScoped<IDataStore<int, InvoiceLineModel>, InvoiceLineStore>();
            
            services.AddScoped(x => (x.GetRequiredService<IDataStore<int, InvoiceLineModel>>() as IInvoiceLineStore)!);

            services.AddScoped<IValidator<InvoiceLine>, InvoiceLineValidator>();


            return services;
        }
    }

    public class ChinookMappingProfile : Profile
    {
        public ChinookMappingProfile()
        {
            CreateMap<Customer, CustomerModel>()
                .ForMember(dest=>dest.SupportRepName, opt => opt.MapFrom(src => src.SupportRep != null ? $"{src.SupportRep.FirstName} {src.SupportRep.LastName}" : ""))
                .ReverseMap();
            CreateMap<Artist, ArtistModel>()
                .ReverseMap();
            CreateMap<Employee, EmployeeModel>()
                .ReverseMap();

            CreateMap<Invoice, InvoiceModel>()
                .ForMember(dest => dest.InvoiceLines, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerId != 0 ? $"{src.Customer.LastName}, {src.Customer.FirstName}" : ""))
                .ReverseMap()
                    .ForMember(dest => dest.InvoiceLines, opt => opt.Ignore());
            
            

            CreateMap<InvoiceLine, InvoiceLineModel>()
                .ForMember(dest => dest.TrackName, opt => opt.MapFrom(src => $"{src.Track.Name} / {src.Track.Album.Title} / {src.Track.Album.Artist.Name}"))
                .ReverseMap()
                    .ForMember(dest => dest.Track, opt => opt.Ignore())
                ;
            
            CreateMap<Track, TrackModel>()
                .ReverseMap();
            //.ForMember(dest => dest., opt => opt.Ignore());
        }
    }
}