using Microsoft.AspNetCore.Mvc;
using NFeXMLValidator.Interfaces;
using NFeXMLValidator.Services;

namespace NFeXMLValidator.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped(typeof(IXMLValidator), typeof(XMLValidator));

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins("")
                                .AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                    });
            });

            services.AddControllers().AddNewtonsoftJson();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                            .AddNewtonsoftJson
                                (options => { }).AddXmlSerializerFormatters()
                          .AddXmlDataContractSerializerFormatters();

            return services;
        }
    }
}
