namespace IntentArchitect.OpenApi.Import.Configuration;

public class PackageItemTypes
{
    public TypeIdentifier Attribute { get; set; }
    public TypeIdentifier Class { get; set; }
    public TypeIdentifier Folder { get; set; }
    public TypeIdentifier Enum { get; set; }
    public TypeIdentifier EnumLiteral { get; set; }
    public TypeIdentifier Service { get; set; }
    public TypeIdentifier ServiceOperation { get; set; }
    public TypeIdentifier UseCase { get; set; }
    public TypeIdentifier Dto { get; set; }
    public TypeIdentifier DtoField { get; set; }
}