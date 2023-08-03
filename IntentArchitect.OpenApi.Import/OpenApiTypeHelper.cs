using Microsoft.OpenApi.Models;

namespace IntentArchitect.OpenApi.Import;

public static class OpenApiTypeHelper
{
    public const string OpenApiArray = "array";
    public const string OpenApiString = "string";
    public const string OpenApiObject = "object";

    public static bool IsCollection(this OpenApiSchema schema) => schema.Type == OpenApiArray;
    public static bool IsString(this OpenApiSchema schema) => schema.GetSchemaType() == OpenApiString; // why
    public static bool IsObject(this OpenApiSchema schema) => schema.Type == OpenApiObject;
    public static bool IsObject(this string type) => type == OpenApiObject;
    public static bool HasProperties(this OpenApiSchema schema) => schema.Items.Properties.Any();

    public static bool IsEnum(this OpenApiSchema schema)
    {
        var type = schema.GetSchemaType();

        return type == OpenApiString && schema.Enum.Any();
    }

    public static string GetSchemaType(this OpenApiSchema schema)
    {
        if (!schema.IsCollection())
            return schema.Format ?? schema.Type;

        var type = schema.Items.Type;

        if (string.IsNullOrEmpty(schema.Type) || schema.Items.OneOf.Any())
            type = OpenApiObject;

        return type;
    }

    public static bool HasValueConstraints(this OpenApiSchema schema) =>
        !schema.IsEnum() && (schema.Minimum.HasValue || schema.Maximum.HasValue);


    public static bool HasStringConstraints(this OpenApiSchema schema) =>
        !schema.IsEnum() && schema.IsString();
}