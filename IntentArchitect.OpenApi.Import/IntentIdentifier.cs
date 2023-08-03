using Humanizer;

namespace IntentArchitect.OpenApi.Import;

public static class IntentIdentifier
{
    public static string New => Guid.NewGuid().ToString().ToLower();

    public static string AsName(this string name) => name.Pascalize();
}