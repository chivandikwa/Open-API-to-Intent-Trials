using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;

using IntentArchitect.OpenApi.Import.Configuration;

using Microsoft.OpenApi.Models;

namespace IntentArchitect.OpenApi.Import;

public static class IntentTypeCreator
{
    public static KeyValuePair<string, string> CreateEnumeration(
        this KeyValuePair<string, OpenApiSchema> openApiEnumeration, ImportConfiguration configuration,
        PackageModelPersistable package)
    {
        var intentEnumeration = package.AddEnumeration(name: openApiEnumeration.Key, configuration);

        for (var index = 0; index < openApiEnumeration.Value.Enum.Count; index++)
        {
            var openApiEnumerationLiteral =
                (Microsoft.OpenApi.Any.OpenApiPrimitive<string>)openApiEnumeration.Value.Enum[index];

            intentEnumeration.AddLiteral(openApiEnumerationLiteral.Value, index + 1, configuration);
        }

        return new KeyValuePair<string, string>(openApiEnumeration.Key.ToLowerInvariant(), intentEnumeration.Id);
    }



    public static string CreateService(this OpenApiTag openApiTag, ImportConfiguration configuration,
        PackageModelPersistable package)
    {
        var service = package.AddService(openApiTag.Name, configuration);

        return service.Id;
    }

    public static string CreateFolder(this PackageModelPersistable package, ImportConfiguration configuration, string name)
    {
        var folderId = package.AddFolder(name.AsName(), configuration);

        return folderId.Id;
    }

    public static void CreateUseCase(this KeyValuePair<OperationType, OpenApiOperation> openApiOperation, ImportConfiguration configuration,
        PackageModelPersistable package)
    {
        package.AddUseCase(openApiOperation, configuration);
    }

    public static void CreateServiceOperation(this KeyValuePair<OperationType, OpenApiOperation> openApiOperation, ImportConfiguration configuration,
        PackageModelPersistable package, string serviceId)
    {
        package.AddServiceOperation(openApiOperation, configuration, serviceId);
    }

    public static void CreateClassProperty(this ElementPersistable intentClass, string className, KeyValuePair<string, OpenApiSchema> property,
        ImportConfiguration configuration,
        PackageModelPersistable package, Dictionary<string, string> openApiEnumNameToIntentIdMap,
        Dictionary<string, string> openApiClassNameToIntentIdMap, string dtoFolderId, PackageModelPersistable servicesPackage)
    {
        var propertyReference = property.Value.Reference is null ? property.Key : property.Value.Reference.Id;
        var isCollection = property.Value.IsCollection();
        var type = property.Value.Format ?? property.Value.Type;

        var isAssociation = !isCollection
            ? type.IsObject() && property.Value.Properties.Any()
            : property.Value.Items.IsObject() && property.Value.HasProperties();

        if (!isAssociation)
        {
            intentClass.AddClassProperty(property.Value, property.Key, openApiEnumNameToIntentIdMap, package,
                configuration);

            return;
        }

        if (!isCollection)
        {
            var classId = CreateClass(new KeyValuePair<string, OpenApiSchema>(propertyReference, new OpenApiSchema
            {
                Properties = property.Value.Properties
            }), configuration, package, openApiEnumNameToIntentIdMap, openApiClassNameToIntentIdMap, dtoFolderId, servicesPackage);

            package.AddClassAssociation(sourceClassId: classId, targetClassId: intentClass.Id,
                sourceName: className, targetName: property.Key);
        }
        else
        {
            if (property.Value.Items.IsObject() && property.Value.HasProperties())
            {
                var arrayReference = property.Value.Items.Reference is null
                    ? property.Key
                    : property.Value.Items.Reference.Id;

                var classId = CreateClass(new KeyValuePair<string, OpenApiSchema>(arrayReference,
                    new OpenApiSchema
                    {
                        Properties = property.Value.Items.Properties
                    }), configuration, package, openApiEnumNameToIntentIdMap, openApiClassNameToIntentIdMap, dtoFolderId, servicesPackage);

                package.AddClassAssociation(sourceClassId: classId, targetClassId: intentClass.Id,
                    sourceName: className, targetName: property.Key);
            }
        }
    }


    public static void CreateDtoProperty(this ElementPersistable intentDto, string className, KeyValuePair<string, OpenApiSchema> property,
     ImportConfiguration configuration,
     PackageModelPersistable package, Dictionary<string, string> openApiEnumNameToIntentIdMap,
     Dictionary<string, string> openApiClassNameToIntentIdMap, string dtoFolderId, PackageModelPersistable servicesPackage)
    {
        var propertyReference = property.Value.Reference is null ? property.Key : property.Value.Reference.Id;
        var isCollection = property.Value.IsCollection();
        var type = property.Value.Format ?? property.Value.Type;

        var isAssociation = !isCollection
            ? type.IsObject() && property.Value.Properties.Any()
            : property.Value.Items.IsObject() && property.Value.HasProperties();

        if (!isAssociation)
        {
            //intentDto.AddDtoProperty(property.Value, property.Key, openApiEnumNameToIntentIdMap, package,
            //    configuration);

            intentDto.IsMapped = true;

            intentDto.Mapping = new MappingModelPersistable
            {
                AutoSyncTypeReference = true,
                MappingSettingsId = intentDto.Mapping.MappingSettingsId,
                Path = new List<MappedPathTargetPersistable>
                {
                    new MappedPathTargetPersistable
                    {
                        Id = IntentIdentifier.New,
                        Name = property.Key.AsName(),
                        Specialization = "Attribute",
                        Type = ElementType.Element
                    }
                }
            };
            return;
        }

      
    }

    public static string CreateClass(
        this KeyValuePair<string, OpenApiSchema> openApiClass, ImportConfiguration configuration,
        PackageModelPersistable package, Dictionary<string, string> openApiEnumNameToIntentIdMap,
        Dictionary<string, string> openApiClassNameToIntentIdMap, string dtoFolderId, PackageModelPersistable servicesPackage)
    {
        var className = openApiClass.Value.Reference is null ? openApiClass.Key : openApiClass.Value.Reference.Id;

        if (openApiClassNameToIntentIdMap.ContainsKey(className.ToLowerInvariant()))
        {
            return openApiClassNameToIntentIdMap[className.ToLowerInvariant()];
        }

        var intentClass = package.AddClass(name: className, configuration);
        var intentDto = servicesPackage.AddDto(name: $"{className}Dto", configuration, dtoFolderId, className, intentClass.Id);

        openApiClassNameToIntentIdMap.Add(className.ToLowerInvariant(), intentClass.Id);


        foreach (var property in openApiClass.Value.Properties)
        {
            intentClass.CreateClassProperty(className, property, configuration, package, openApiEnumNameToIntentIdMap,
                openApiClassNameToIntentIdMap, dtoFolderId, servicesPackage);

            intentDto.CreateDtoProperty(className, property, configuration, package, openApiEnumNameToIntentIdMap,
                openApiClassNameToIntentIdMap, dtoFolderId, servicesPackage);
        }

        return intentClass.Id;
    }

    //public static string CreateDto(
    //    this KeyValuePair<string, OpenApiSchema> openApiClass, ImportConfiguration configuration,
    //    PackageModelPersistable package, Dictionary<string, string> openApiEnumNameToIntentIdMap,
    //    Dictionary<string, string> openApiClassNameToIntentIdMap, string folderId)
    //{
    //    var className = openApiClass.Value.Reference is null ? openApiClass.Key : openApiClass.Value.Reference.Id;

    //    if (openApiClassNameToIntentIdMap.ContainsKey(className.ToLowerInvariant()))
    //    {
    //        return openApiClassNameToIntentIdMap[className.ToLowerInvariant()];
    //    }

    //    var intentClass = package.AddDto(name: className, configuration, folderId);


    //    openApiClassNameToIntentIdMap.Add(className.ToLowerInvariant(), intentClass.Id);


    //    //foreach (var property in openApiClass.Value.Properties)
    //    //{
    //    //    intentClass.CreateClassProperty(className, property, configuration, package, openApiEnumNameToIntentIdMap,
    //    //        openApiClassNameToIntentIdMap);
    //    //}

    //    return intentClass.Id;
    //}
}