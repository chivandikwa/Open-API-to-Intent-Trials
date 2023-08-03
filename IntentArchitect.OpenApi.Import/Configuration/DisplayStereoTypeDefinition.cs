namespace IntentArchitect.OpenApi.Import.Configuration;

public class DisplayStereoTypeDefinition
{
    public string Id { get; set; }
    public string PackageId { get; set; }
    public string PackageName { get; set; }
    public bool AddedByDefault { get; set; }
    public string Name { get; set; }
    public StereoTypePropertyDefinition NameProperty { get; set; }
    public StereoTypePropertyDefinition DescriptionProperty { get; set; }
}