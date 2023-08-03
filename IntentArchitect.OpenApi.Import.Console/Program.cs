using CommandLine;

using IntentArchitect.OpenApi.Import.Configuration;

using Microsoft.Extensions.Configuration;

namespace IntentArchitect.OpenApi.Import.Console;

internal class Program
{
    static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsedAsync(async options =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build()
                    .Get<ImportConfiguration>();

                configuration!.PackageName = options.PackageName;
                configuration.SourceFilePath = options.SourceFilePath;
                configuration.OpenApiFormat = options.OpenApiFormat;

                await new OpenApiImporter(configuration).Import();
            });
    }

}