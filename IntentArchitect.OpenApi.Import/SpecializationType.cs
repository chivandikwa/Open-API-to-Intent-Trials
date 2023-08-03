namespace IntentArchitect.OpenApi.Import;

public class SpecializationType
{
    public SpecializationType(string name, string id)
    {
        Name = name;
        Id = id;
    }
    public string Name { get; set; }
    public string Id { get; set; }
}