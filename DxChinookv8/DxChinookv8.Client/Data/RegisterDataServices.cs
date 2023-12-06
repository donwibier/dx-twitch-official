using DxBlazor.UI;
using DxChinook.Data;
using DxChinook.Data.Models;
using DxChinookv8.Client.Data;
using FluentValidation;

namespace DxChinookWASM.Client.Services
{
    public static class RegisterDataServices
    {
        public static IServiceCollection RegisterClientDataServices(this IServiceCollection services)
        {
            services.AddTransient<IDevExtremeLoader, DevExtremeClientLoader>();

            services.AddScoped<IDataStore<int, ArtistModel>, ArtistApiStore>();
            services.AddScoped<IDataStore<int, CustomerModel>, CustomerApiStore>();
            services.AddScoped<IDataStore<int, EmployeeModel>, EmployeeApiStore>();
            services.AddScoped<IDataStore<int, InvoiceModel>, InvoiceApiStore>();
            services.AddScoped<IDataStore<int, InvoiceLineModel>, InvoiceLineApiStore>();
            services.AddScoped(x => (x.GetRequiredService<IDataStore<int, InvoiceLineModel>>() as IInvoiceLineStore)!);

            return services;
        }

    }
}
