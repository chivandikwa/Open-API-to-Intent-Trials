using CommandLine;

namespace IntentArchitect.OpenApi.Import.Console;

public class CommandLineOptions
{
    [Option(shortName: 'p', longName: "package-name", Required = false, HelpText = "The name of the package to create", Default = "Package")]
    public string PackageName { get; set; }

    [Option(shortName: 's', longName: "source-file-path", Required = true, HelpText = "The path to the Open API spec to import")]
    public string SourceFilePath { get; set; }

    [Option(shortName: 'f', longName: "open-api-format", Required = false, HelpText = "The format of the Open API spec", Default = OpenApiFormat.YamlV3)]
    public OpenApiFormat OpenApiFormat { get; set; }
}