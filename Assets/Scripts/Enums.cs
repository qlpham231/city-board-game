using System.ComponentModel;
using System.Reflection;
using System;

public static class EnumExtensions
{
    public static string GetDescription<T>(this T enumValue) where T : Enum
    {
        var field = typeof(T).GetField(enumValue.ToString());

        if (field is null)
            return enumValue.ToString(); // Return enum name if no field found

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description ?? enumValue.ToString();
    }
}