using Avalonia.Data.Converters;

namespace Sightful.Avalonia.Controls.PropertyGrid;

internal static class Converters
{
	public static FuncValueConverter<string, int> RestrictMaxLinesToSingleIfEmptyString { get; } =
		new(str => string.IsNullOrEmpty(str) ? 1 : 0);
}