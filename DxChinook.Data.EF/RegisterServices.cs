using DxChinook.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DxChinook.Data.EF
{
    public static class RegisterServices
    {
        public static IServiceCollection RegisterDataServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ChinookContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);

            return services;
        }
    }
}