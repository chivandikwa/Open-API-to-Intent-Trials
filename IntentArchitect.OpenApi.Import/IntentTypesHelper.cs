using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;

using IntentArchitect.OpenApi.Import.Configuration;

using Microsoft.OpenApi.Models;

namespace IntentArchitect.OpenApi.Import;



public static class IntentTypesHelper
{
    public static string ToIntentType(this string openApiDataType, ImportConfiguration configuration) =>
        openApiDataType.ToLower() switch
        {
            "string" => configuration.TypeDefinitions.String,
            "byte" => configuration.TypeDefinitions.String,
            "password" => configuration.TypeDefinitions.String,
            "email" => configuration.TypeDefinitions.String,
            "hostname" => configuration.TypeDefinitions.String,
            "ipv4" => configuration.TypeDefinitions.String,
            "ipv6" => configuration.TypeDefinitions.String,
            "object" => configuration.TypeDefinitions.Object,
            "int32" => configuration.TypeDefinitions.Int,
            "integer" => configuration.TypeDefinitions.Int,
            "int64" => configuration.TypeDefinitions.Long,
            "float" => configuration.TypeDefinitions.Decimal,
            "number" => configuration.TypeDefinitions.Decimal,
            "double" => configuration.TypeDefinitions.Decimal,
            "boolean" => configuration.TypeDefinitions.Bool,
            "date" => configuration.TypeDefinitions.Date,
            "date-time" => configuration.TypeDefinitions.DatetimeOffset,
            "uuid" => configuration.TypeDefinitions.Guid,
            _ => configuration.TypeDefinitions.String
        };

    public static ElementPersistable AddService(this PackageModelPersistable package, string name,
        ImportConfiguration configuration)
    {
        var service = new ElementPersistable
        {
            Id = IntentIdentifier.New,
            Name = name.AsName(),
            ParentFolderId = package.Id,
            SpecializationTypeId = configuration.PackageItemTypes.Service.Id,
            SpecializationType = configuration.PackageItemTypes.Service.Name,
        };

        package.Classes.Add(service);

        return service;
    }
    public static ElementPersistable AddFolder(this PackageModelPersistable package, string name,
        ImportConfiguration configuration)
    {
        var service = new ElementPersistable
        {
            Id = IntentIdentifier.New,
            Name = name.AsName(),
            ParentFolderId = package.Id,
            SpecializationTypeId = configuration.PackageItemTypes.Folder.Id,
            SpecializationType = configuration.PackageItemTypes.Folder.Name,
        };

        package.Classes.Add(service);

        return service;
    }


    public static void AddServiceOperation(this PackageModelPersistable package, KeyValuePair<OperationType, OpenApiOperation> openApiOperation,
        ImportConfiguration configuration, string serviceId)
    {
        var service = new ElementPersistable
        {
            Id = IntentIdentifier.New,
            Name = openApiOperation.Value.OperationId.AsName(),
            ParentFolderId = serviceId,
            SpecializationTypeId = configuration.PackageItemTypes.ServiceOperation.Id,
            SpecializationType = configuration.PackageItemTypes.ServiceOperation.Name,
        };

        package.Classes.Add(service);
    }


    public static void AddUseCase(this PackageModelPersistable package, KeyValuePair<OperationType, OpenApiOperation> openApiOperation,
        ImportConfiguration configuration)
    {
        var service = new ElementPersistable
        {
            Id = IntentIdentifier.New,
            Name = openApiOperation.Value.OperationId.AsName(),
            ParentFolderId = package.Id,
            SpecializationTypeId = configuration.PackageItemTypes.UseCase.Id,
            SpecializationType = configuration.PackageItemTypes.UseCase.Name,
        };

        package.Classes.Add(service);
    }

    public static ElementPersistable AddEnumeration(this PackageModelPersistable package, string name,
        ImportConfiguration configuration)
    {
        var enumeration = new ElementPersistable
        {
            Id = IntentIdentifier.New,
            Name = name.AsName(),
            ParentFolderId = package.Id,
            SpecializationTypeId = configuration.PackageItemTypes.Enum.Id,
            SpecializationType = configuration.PackageItemTypes.Enum.Name,
        };

        package.Classes.Add(enumeration);

        return enumeration;
    }

    public static ElementPersistable AddClass(this PackageModelPersistable package, string name,
        ImportConfiguration configuration)
    {
        var @class = new ElementPersistable
        {
            Id = IntentIdentifier.New,
            ParentFolderId = package.Id,
            Name = name.AsName(),
            SpecializationTypeId = configuration.PackageItemTypes.Class.Id,
            SpecializationType = configuration.PackageItemTypes.Class.Name,
        };

        package.Classes.Add(@class);

        return @class;
    }

    public static ElementPersistable AddDto(this PackageModelPersistable package, string name,
        ImportConfiguration configuration, string folderId, string targetName, string targetId)
    {
        var @class = new ElementPersistable
        {
            Id = IntentIdentifier.New,
            ParentFolderId = folderId,
            Name = name.AsName(),
            SpecializationTypeId = configuration.PackageItemTypes.Dto.Id,
            SpecializationType = configuration.PackageItemTypes.Dto.Name,
            IsMapped = true,
            Mapping = new MappingModelPersistable
            {
                AutoSyncTypeReference = true,
                MappingSettingsId = IntentIdentifier.New,
                Path = new List<MappedPathTargetPersistable>
                {
                    new MappedPathTargetPersistable
                    {
                        Id = targetId, 
                        Name = targetName,
                        Specialization = "Class",
                        Type = ElementType.Element
                    }
                }
            }
        };

        package.Classes.Add(@class);

        return @class;
    }

    public static void AddClassAssociation(this PackageModelPersistable package, string sourceClassId,
        string targetClassId, string sourceName, string targetName) =>
        package.Associations.Add(new AssociationPersistable
        {
            AssociationType = "Association",
            AssociationTypeId = "eaf9ed4e-0b61-4ac1-ba88-09f912c12087",
            Id = IntentIdentifier.New,
            TargetEnd = new AssociationEndPersistable
            {
                Id = IntentIdentifier.New,
                Name = targetName.AsName(),
                SpecializationType = "Association Target End",
                SpecializationTypeId = "0a66489f-30aa-417b-a75d-b945863366fd",
                TypeReference = new TypeReferencePersistable
                {
                    Id = IntentIdentifier.New,
                    Stereotypes = new List<StereotypePersistable>(),
                    GenericTypeParameters = new List<TypeReferencePersistable>(),
                    TypeId = sourceClassId,
                    TypePackageId = package.Id,
                    TypePackageName = package.Name,
                },
            },
            SourceEnd = new AssociationEndPersistable
            {
                Id = IntentIdentifier.New,
                Name = sourceName.AsName(),
                SpecializationType = "Association Source End",
                SpecializationTypeId = "8d9d2e5b-bd55-4f36-9ae4-2b9e84fd4e58",
                TypeReference = new TypeReferencePersistable
                {
                    Id = IntentIdentifier.New,
                    Stereotypes = new List<StereotypePersistable>(),
                    GenericTypeParameters = new List<TypeReferencePersistable>(),
                    TypeId = targetClassId,
                    TypePackageId = package.Id,
                    TypePackageName = package.Name,
                    IsNavigable = false
                },
            }
        });


    public static void AddNumericStereoType(this ElementPersistable intentClassProperty, OpenApiSchema openApiClassProperty)
    {
        var minimum = openApiClassProperty.Minimum;
        var maximum = openApiClassProperty.Maximum;

        var exclusiveMinimum = openApiClassProperty.ExclusiveMinimum;
        var exclusiveMaximum = openApiClassProperty.ExclusiveMaximum;

        intentClassProperty.Stereotypes.Add(new StereotypePersistable
        {
            DefinitionId = "0de75b2a-05b2-46ea-b8fc-28cefbf03510",
            DefinitionPackageId = "2885a28e-981b-40a8-93f2-25be5dc0a65e",
            DefinitionPackageName = "BSC.Modelers.Domain",
            Name = "Numeric",
            Properties = new List<StereotypePropertyPersistable>
            {
                new()
                {
                    IsActive = true,
                    Name = "MinValue",
                    Id = "1ae5b27a-bbf0-466c-9f2b-6387f6c38c0a",
                    Value = minimum.HasValue
                        ? $"{minimum.GetNumericConstraint(exclusiveMinimum, 1)}"
                        : string.Empty
                },
                new()
                {
                    IsActive = true,
                    Name = "MaxValue",
                    Id = "440a3fcb-f636-45e1-91cc-51a25d8325ae",
                    Value = maximum.HasValue
                        ? $"{maximum.GetNumericConstraint(exclusiveMaximum, -1)}"
                        : string.Empty,
                }
            }
        });
    }

    public static void AddTextConstraintsStereoType(this ElementPersistable intentClassProperty, OpenApiSchema openApiClassProperty)
    {
        var maxLength = openApiClassProperty.MaxLength;

        intentClassProperty.Stereotypes.Add(new StereotypePersistable
        {
            DefinitionId = "6347286E-A637-44D6-A5D7-D9BE5789CA7A",
            DefinitionPackageId = "AF8F3810-745C-42A2-93C8-798860DC45B1",
            DefinitionPackageName = "Intent.Metadata.RDBMS",
            Name = "Text Constraints",
            Properties = new List<StereotypePropertyPersistable>
            {
                new()
                {
                    IsActive = true,
                    Name = "SQL Data Type",
                    Id = "1288cfcd-ee51-437e-9713-73b80118f026",
                    Value = "DEFAULT"
                },
                new()
                {
                    IsActive = true,
                    Name = "MaxLength",
                    Id = "A04CC24D-81FB-4EA2-A34A-B3C58E04DCFD",
                    Value = maxLength.HasValue
                        ? $"{maxLength.GetNumericConstraint(exclusive: false)}"
                        : string.Empty,
                },
                new()
                {
                    IsActive = true,
                    Name = "IsUnicode",
                    Id = "67EC4CF4-7706-4B39-BC7C-DF539EE2B0AF",
                    Value = "false"
                }
            }
        });
    }

    public static void AddClassProperty(this ElementPersistable @class, OpenApiSchema openApiClassProperty,
        string name, Dictionary<string, string> openApiEnumNameToIntentIdMap, PackageModelPersistable package,
        ImportConfiguration configuration)
    {
        var type = openApiClassProperty.GetSchemaType();
        var isCollection = openApiClassProperty.IsCollection();
        var isEnum = openApiClassProperty.IsEnum();

        var intentClassProperty = new ElementPersistable
        {
            Id = IntentIdentifier.New,
            Name = name.AsName(),
            SpecializationTypeId = configuration.PackageItemTypes.Attribute.Id,
            SpecializationType = configuration.PackageItemTypes.Attribute.Name,
            Stereotypes = new List<StereotypePersistable>(),
            TypeReference = new TypeReferencePersistable
            {
                Id = IntentIdentifier.New,
                IsNullable = openApiClassProperty.Nullable,
                IsCollection = isCollection,
                Stereotypes = new List<StereotypePersistable>(),
                GenericTypeParameters = new List<TypeReferencePersistable>(),
                TypeId = isEnum
                    ? openApiEnumNameToIntentIdMap[openApiClassProperty.Reference.Id.ToLower()]
                    : type.ToIntentType(configuration),
                TypePackageId = isEnum ? package.Id : null,
                TypePackageName = isEnum ? package.Name : null,
            },
        };

        if (openApiClassProperty.HasValueConstraints())
            intentClassProperty.AddNumericStereoType(openApiClassProperty);

        if (openApiClassProperty.HasStringConstraints())
            intentClassProperty.AddTextConstraintsStereoType(openApiClassProperty);

        @class.ChildElements.Add(intentClassProperty);
    }

    public static void AddDtoProperty(this ElementPersistable @class, OpenApiSchema openApiClassProperty,
        string name, Dictionary<string, string> openApiEnumNameToIntentIdMap, PackageModelPersistable domainPackage,
        ImportConfiguration configuration)
    {
        var type = openApiClassProperty.GetSchemaType();
        var isCollection = openApiClassProperty.IsCollection();
        var isEnum = openApiClassProperty.IsEnum();

        var intentClassProperty = new ElementPersistable
        {
            Id = IntentIdentifier.New,
            Name = name.AsName(),
            SpecializationTypeId = configuration.PackageItemTypes.DtoField.Id,
            SpecializationType = configuration.PackageItemTypes.DtoField.Name,
            Stereotypes = new List<StereotypePersistable>(),
            TypeReference = new TypeReferencePersistable
            {
                Id = IntentIdentifier.New,
                IsNullable = openApiClassProperty.Nullable,
                IsCollection = isCollection,
                Stereotypes = new List<StereotypePersistable>(),
                GenericTypeParameters = new List<TypeReferencePersistable>(),
                TypeId = isEnum
                    ? openApiEnumNameToIntentIdMap[openApiClassProperty.Reference.Id.ToLower()]
                    : type.ToIntentType(configuration),
                TypePackageId = isEnum ? domainPackage.Id : null,
                TypePackageName = isEnum ? domainPackage.Name : null,
            },
        };

        //if (openApiClassProperty.HasValueConstraints())
        //    intentClassProperty.AddNumericStereoType(openApiClassProperty);

        //if (openApiClassProperty.HasStringConstraints())
        //    intentClassProperty.AddTextConstraintsStereoType(openApiClassProperty);

        @class.ChildElements.Add(intentClassProperty);
    }

    public static void AddLiteral(this ElementPersistable enumeration, string name, int value,
        ImportConfiguration configuration) =>
        enumeration.ChildElements.Add(new ElementPersistable
        {
            Id = IntentIdentifier.New,
            Name = name.AsName(),
            ParentFolderId = enumeration.Id,
            SpecializationTypeId = configuration.PackageItemTypes.EnumLiteral.Id,
            SpecializationType = configuration.PackageItemTypes.EnumLiteral.Name,
            Value = value.ToString(),
            Stereotypes = new List<StereotypePersistable>
            {
                new()
                {
                    DefinitionId = configuration.EnumLiteralStereoTypes.Display.Id,
                    DefinitionPackageId = configuration.EnumLiteralStereoTypes.Display.PackageId,
                    DefinitionPackageName = configuration.EnumLiteralStereoTypes.Display.PackageName,
                    AddedByDefault = configuration.EnumLiteralStereoTypes.Display.AddedByDefault,
                    Name = configuration.EnumLiteralStereoTypes.Display.Name,
                    Properties = new List<StereotypePropertyPersistable>
                    {
                        new()
                        {
                            IsActive = configuration.EnumLiteralStereoTypes.Display.NameProperty.IsActive,
                            Name = configuration.EnumLiteralStereoTypes.Display.NameProperty.Name,
                            Id = configuration.EnumLiteralStereoTypes.Display.NameProperty.Id,
                            Value = name.Any(char.IsWhiteSpace) ? name : null
                        },
                        new()
                        {
                            IsActive = configuration.EnumLiteralStereoTypes.Display.DescriptionProperty.IsActive,
                            Name = configuration.EnumLiteralStereoTypes.Display.DescriptionProperty.Name,
                            Id = configuration.EnumLiteralStereoTypes.Display.DescriptionProperty.Id,
                        }
                    }
                },
                new()
                {
                    DefinitionId = configuration.EnumLiteralStereoTypes.SortOrder.Id,
                    DefinitionPackageId = configuration.EnumLiteralStereoTypes.SortOrder.PackageId,
                    DefinitionPackageName = configuration.EnumLiteralStereoTypes.SortOrder.PackageName,
                    AddedByDefault = configuration.EnumLiteralStereoTypes.SortOrder.AddedByDefault,
                    Name = configuration.EnumLiteralStereoTypes.SortOrder.Name,
                    Properties = new List<StereotypePropertyPersistable>
                    {
                        new()
                        {
                            IsActive = configuration.EnumLiteralStereoTypes.SortOrder.ValueProperty.IsActive,
                            Name = configuration.EnumLiteralStereoTypes.SortOrder.ValueProperty.Name,
                            Id = configuration.EnumLiteralStereoTypes.SortOrder.ValueProperty.Id,
                            Value = ""
                        }
                    }
                }
            }
        });
}