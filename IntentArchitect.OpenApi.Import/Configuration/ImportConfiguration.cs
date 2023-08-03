namespace IntentArchitect.OpenApi.Import.Configuration;

public class ImportConfiguration
{
    public TypeDefinitions TypeDefinitions { get; set; }
    public PackageTypes PackageTypes { get; set; }
    public PackageItemTypes PackageItemTypes { get; set; }
    public IEnumerable<PackageReference> DomainPackageReferences { get; set; }
    public IEnumerable<PackageReference> ServicesPackageReferences { get; set; }
    public OpenApiFormat OpenApiFormat { get; set; }
    public string SourceFilePath { get; set; }
    public string PackageName { get; set; }
    public EnumLiteralStereoTypes EnumLiteralStereoTypes { get; set; }
}

//public class StereoTypeDefinition
//{
//    public string Id { get; set; }
//    public string PackageId { get; set; }
//    public string PackageName { get; set; }
//    public bool AddedByDefault { get; set; }
//    public string Name { get; set; }
//    public IEnumerable<StereoTypePropertyDefinition> Properties { get; set; }
//}