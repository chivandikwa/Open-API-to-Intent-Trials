using System.IO;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Microsoft.OpenApi.Models;

using Microsoft.OpenApi.Readers;
using IntentArchitect.OpenApi.Import.Configuration;
using SharpYaml.Tokens;

namespace IntentArchitect.OpenApi.Import;

//. array oneOf

//revolvingFacilityDetails - object

// check required properties for generating services
// You can use the readOnly and writeOnly keywords to mark specific properties as read-only or write-only. This is useful, for example, when GET returns more properties than used in POST – you can use the same schema in both GET and POST and mark the extra properties as readOnly. readOnly properties are included in responses but not in requests, and writeOnly properties may be sent in requests but not in responses

// test AnyValue: {}

public class OpenApiImporter
{
    private readonly ImportConfiguration _configuration;
    private readonly Dictionary<string, string> _openApiEnumNameToIntentIdMap = new();
    private readonly Dictionary<string, string> _openApiClassNameToIntentIdMap = new();
    private readonly PackageModelPersistable _domainPackage;
    private readonly PackageModelPersistable _servicesPackage;
    private OpenApiDocument _openApiDocument;

    public OpenApiImporter(ImportConfiguration configuration)
    {
        _configuration = configuration;

        _domainPackage = CreateDomainPackage();
        _servicesPackage = CreateServicesPackage();
    }

    public async Task Import()
    {
        var stream = new FileStream("spec.yaml", FileMode.Open);

        var spec = (await new OpenApiStreamReader().ReadAsync(stream)).OpenApiDocument;

        var package = PackageModelPersistable.Load(
            @"C:\dev\bsc-lib\cs-bsc-net-to-intent-util\NetIntentModulePort\NetIntentModulePort\bin\Debug\net7.0\Domain\Exported.pkg.config");

        _openApiDocument = await ParseOpenApiSpec();

        CreateDomainPackageEntities();
        CreateServicesPackageEntities();

        _domainPackage.Save();
        _servicesPackage.Save();
    }

    private PackageModelPersistable CreateServicesPackage()
    {
        var package = PackageModelPersistable
            .CreateEmpty(_configuration.PackageTypes.ServicesPackage.Id, _configuration.PackageTypes.ServicesPackage.Name,
                IntentIdentifier.New, $"{_configuration.PackageName}.Services");

        package.AbsolutePath = Path.Combine(Environment.CurrentDirectory, "Services",
            $"{_configuration.PackageName}.Services.{Constants.PackageExtension}");

        AddServicesPackageReferences(package);

        return package;
    }

    private PackageModelPersistable CreateDomainPackage()
    {
        var package = PackageModelPersistable
            .CreateEmpty(_configuration.PackageTypes.DomainPackage.Id, _configuration.PackageTypes.DomainPackage.Name,
                IntentIdentifier.New, _configuration.PackageName);

        package.AbsolutePath = Path.Combine(Environment.CurrentDirectory, "Domain",
            $"{_configuration.PackageName}.{Constants.PackageExtension}");

        AddDomainPackageReferences(package);

        return package;
    }

    private async Task<OpenApiDocument> ParseOpenApiSpec()
    {
        var stream = new FileStream(_configuration.SourceFilePath, FileMode.Open);

        return _configuration.OpenApiFormat switch
        {
            OpenApiFormat.JsonV2 => (await new OpenApiStreamReader().ReadAsync(stream)).OpenApiDocument,
            OpenApiFormat.YamlV3 => (await new OpenApiStreamReader().ReadAsync(stream)).OpenApiDocument,
            _ => throw new InvalidOperationException(
                "Only ingests OpenAPI JSON and YAML documents in both V2 and V3 format")
        };
    }

    private void CreateServicesPackageEntities()
    {
        if (!_openApiDocument.Tags.Any())
            throw new InvalidOperationException("Tag required");

        var tagToServiceIdMapping = new Dictionary<string, string>();

        foreach (var tag in _openApiDocument.Tags)
        {
            var serviceId = tag.CreateService(_configuration, _servicesPackage);

            tagToServiceIdMapping.Add(tag.Name.ToLowerInvariant(), serviceId);
        }

        foreach (var path in _openApiDocument.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                var serviceId = tagToServiceIdMapping[operation.Value.Tags.First().Name.ToLowerInvariant()];

                operation.CreateServiceOperation(_configuration, _servicesPackage, serviceId);

                operation.CreateUseCase(_configuration, _servicesPackage);

                //foreach (var response in operation.Value.Responses)
                //{
                //    if (response.Value.Content.Any())
                //    {
                //        var content = response.Value.Content.First().Value;
                //        if (content.Schema.Reference is not null)
                //        {
                //            var schema = new KeyValuePair<string, OpenApiSchema>(content.Schema.Reference.Id, content.Schema);

                //            schema.CreateDto(_configuration, _servicesPackage, _openApiEnumNameToIntentIdMap,
                //                _openApiClassNameToIntentIdMap, folderId);
                //        }
                //    }

                //}

                //if (operation.Value.RequestBody is null || !operation.Value.RequestBody.Content.ContainsKey("application/json"))
                //    continue;

                //var xx = operation.Value.RequestBody.Content["application/json"];

                //if (xx.Schema.Reference is not null)
                //{
                //    var schema = new KeyValuePair<string, OpenApiSchema>(xx.Schema.Reference.Id, xx.Schema);

                //    schema.CreateDto(_configuration, _servicesPackage, _openApiEnumNameToIntentIdMap,
                //        _openApiClassNameToIntentIdMap, folderId);
                //}

            }
        }

    }

    private void CreateDomainPackageEntities()
    {
        var enumerations = _openApiDocument.Components.Schemas
            .Where(x => x.Value.Enum.Any())
            .ToArray();

        var customObjects = _openApiDocument.Components.Schemas
            .Where(x => x.Value.Properties.Any())
            .ToArray();

        foreach (var enumeration in enumerations)
        {
            var (key, value) = enumeration.CreateEnumeration(_configuration, _domainPackage);

            _openApiEnumNameToIntentIdMap.Add(key, value);
        }

        var dtoFolderId = _servicesPackage.CreateFolder(_configuration, "contracts");

        foreach (var customObject in customObjects)
        {
            customObject.CreateClass(_configuration, _domainPackage, _openApiEnumNameToIntentIdMap,
                _openApiClassNameToIntentIdMap, dtoFolderId, _servicesPackage);
        }
    }

    private void AddDomainPackageReferences(PackageModelPersistable package)
    {
        foreach (var reference in _configuration.DomainPackageReferences)
        {
            package.References.Add(new PackageReferenceModel
            {
                PackageId = reference.Id,
                Module = reference.Name,
                IsExternal = reference.IsExternal
            });
        }
    }

    private void AddServicesPackageReferences(PackageModelPersistable package)
    {
        foreach (var reference in _configuration.ServicesPackageReferences)
        {
            package.References.Add(new PackageReferenceModel
            {
                PackageId = reference.Id,
                Module = reference.Name,
                IsExternal = reference.IsExternal
            });
        }

        package.References.Add(new PackageReferenceModel
        {
            PackageId = _domainPackage.Id,
            IsExternal = true,
            AbsolutePath = _domainPackage.AbsolutePath,
            Name = _domainPackage.Name
        });
    }
}