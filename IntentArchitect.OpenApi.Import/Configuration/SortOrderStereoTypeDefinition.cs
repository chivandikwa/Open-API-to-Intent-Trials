namespace IntentArchitect.OpenApi.Import.Configuration;

public class SortOrderStereoTypeDefinition
{
    public string Id { get; set; }
    public string PackageId { get; set; }
    public string PackageName { get; set; }
    public bool AddedByDefault { get; set; }
    public string Name { get; set; }
    public StereoTypePropertyDefinition ValueProperty { get; set; }
}