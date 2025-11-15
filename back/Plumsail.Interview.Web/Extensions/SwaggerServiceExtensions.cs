using Microsoft.OpenApi.Models;

using System.Reflection;

namespace Plumsail.Interview.Web.Extensions;

public static class SwaggerServiceExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Plumsail Interview API",
                Description = "API for Plumsail Interview application",
                Contact = new OpenApiContact
                {
                    Name = "Plumsail",
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });
    }

    public static void UseSwaggerWithUI(this IApplicationBuilder app)
    {
        if (app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Plumsail Interview API v1");
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
            });
        }
    }
}