using FileShareProcessor.Service;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

[assembly: FunctionsStartup(typeof(FileShareProcessor.Startup))]
namespace FileShareProcessor
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder.AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false).AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDurableClientFactory();
            builder.Services.AddOptions<FunctionAppSettings>().Configure<IConfiguration>((settings, configuration) => configuration.GetSection("FunctionAppSettings").Bind(settings));
            builder.Services.AddSingleton<IPatternService, PatternService>();
            builder.Services.AddSingleton<IFileShareService, FileShareService>();
            builder.Services.AddSingleton<IFileProcessService, FileProcessService>();
        }
    }
}
