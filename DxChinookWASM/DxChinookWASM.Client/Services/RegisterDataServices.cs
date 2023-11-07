using DxBlazor.UI;
using DxChinook.Data;
using DxChinook.Data.Models;
using FluentValidation;

namespace DxChinookWASM.Client.Services
{
    public static class RegisterDataServices
    {
        public static IServiceCollection RegisterClientDataServices(this IServiceCollection services)
        {
            services.AddTransient<IDevExtremeLoader, DevExtremeClientLoader>();
            services.AddScoped<IDataStore<int, CustomerModel>, CustomerApiStore>();            

            return services;
        }

    }
}
