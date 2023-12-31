﻿using DxChinook.Data.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxChinook.Data
{
    public static class RegisterServices
    {
        public static IServiceCollection RegisterModelValidators(this IServiceCollection services)
        {            
            services.AddScoped<IValidator<CustomerModel>, CustomerModelValidator>();
            services.AddScoped<IValidator<ArtistModel>, ArtistModelValidator>();
            services.AddScoped<IValidator<EmployeeModel>, EmployeeModelValidator>();

            services.AddScoped<IValidator<InvoiceModel>, InvoiceModelValidator>();
            services.AddScoped<IValidator<InvoiceLineModel>, InvoiceLineModelValidator>();
            services.AddScoped<IValidator<TrackModel>, TrackModelValidator>();

            return services;
        }
    }
}
