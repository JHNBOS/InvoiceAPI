using System;
using System.IO;
using System.Linq;
using System.Reflection;
using InvoiceWebApp.Components.Services;
using InvoiceWebApp.Components.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Swashbuckle.AspNetCore.Swagger;

namespace InvoiceWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the Swagger generator, defining one or more Swagger documents  
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info { Title = "Invoice App API", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // CORS
            services.AddCors(options => {
                options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

			// Singleton repos
			services.AddSingleton<IAddressRepository, AddressRepository>();
			services.AddSingleton<IDebtorHasAddressRepository, DebtorHasAddressRepository>();
			services.AddSingleton<IDebtorRepository, DebtorRepository>();
			services.AddSingleton<IInvoiceItemRepository, InvoiceItemRepository>();
			services.AddSingleton<IInvoiceRepository, InvoiceRepository>();
			services.AddSingleton<IRoleRepository, RoleRepository>();
			services.AddSingleton<ISettingRepository, SettingRepository>();
			services.AddSingleton<IUserRepository, UserRepository>();

			services.AddMvc().AddSessionStateTempDataProvider().AddControllersAsServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider svp)
        {
            app.Use(async (context, next) => {
                await next();
                if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value) &&
                     !context.Request.Path.Value.StartsWith("/api/"))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseMvcWithDefaultRoute();

            app.UseSwaggerUI(c => {
                c.RoutePrefix = "api";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice Web API");
            });

            app.UseCors("AllowAll");

            app.UseMvc(routes => {
                routes.MapRoute(
                  name: "api",
                  template: "api/{controller}/{action}/{id?}"
                );
            });
        }
    }
}
