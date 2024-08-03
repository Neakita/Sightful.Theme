using System.Linq;
using Microsoft.CodeAnalysis;

namespace Sightful.Avalonia.Controls.PropertyGrid;

internal static class Extensions
{
	public static T? GetAttributeValue<T>(this ISymbol propertySymbol, string attributeName)
	{
		var attributes = propertySymbol.GetAttributes();
		var attribute = attributes.FirstOrDefault(attribute => attribute.AttributeClass?.Name == attributeName);
		return GetAttributeValue<T>(attribute);
	}

	public static T? GetAttributeValue<T>(this AttributeData? displayNameAttribute)
	{
		return (T?)displayNameAttribute?.ConstructorArguments.FirstOrDefault().Value;
	}

	// Convert "SomeString" to "Some string"
	public static string FormatForDisplay(this string input)
	{
		input = SplitCamelCase(input);
		input = input.Substring(0, 1) + input.Substring(1).ToLower();
		return input;
	}

	public static string SplitCamelCase(this string input)
	{
		return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
	}

	public static string ToPropertyName(this string value)
	{
		if (value[0] == '_')
			value = value.Substring(1);
		if (char.IsLower(value[0]))
			value = char.ToUpper(value[0]) + value.Substring(1);
		return value;
	}
}