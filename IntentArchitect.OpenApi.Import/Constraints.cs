namespace IntentArchitect.OpenApi.Import;

public static class Constraints
{
    public static decimal? GetNumericConstraint(this int? value, bool? exclusive, int multiplier = 1)
    {
        if (value is null)
            return null;

        return exclusive ?? false ? value + 1 * multiplier : value;
    }
    public static decimal? GetNumericConstraint(this decimal? value, bool? exclusive, int multiplier = 1)
    {
        if (value is null)
            return null;

        return exclusive ?? false ? value + 1 * multiplier : value;
    }

}