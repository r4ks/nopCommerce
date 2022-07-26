using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.HumanResource.Areas.Admin.Factories;
using Nop.Plugin.Widgets.HumanResource.Web.Infrastructure;
using Nop.Plugin.Widgets.HumanResource.Services;
using Nop.Plugin.Widgets.HumanResource.Services.ExportImport;
using Nop.Plugin.Widgets.HumanResource.Services.HumanResource;
using Nop.Plugin.Widgets.HumanResource.Services.Installation;
using Nop.Plugin.Widgets.HumanResource.Services.Common;

namespace Nop.Plugin.Widgets.HumanResource.Web.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring services on application startup
    /// </summary>
    public class PluginNopStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });

            //register services and interfaces

            services.AddScoped<IPluginBaseAdminModelFactory, PluginBaseAdminModelFactory>();
            services.AddScoped<IEmployeeModelFactory, EmployeeModelFactory>();
            services.AddScoped<IPluginExportManager, PluginExportManager>();
            services.AddScoped<IPluginImportManager, PluginImportManager>();
            services.AddScoped<IExtraInstallationService, ExtraInstallationService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IEmployeeSettingModelFactory, EmployeeSettingModelFactory>();
            services.AddScoped<IPluginPdfService, PluginPdfService>();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 11;
    }
}