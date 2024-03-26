using Avalonia;
using Avalonia.Data.Converters;

namespace Sightful.Avalonia.Theme;

internal static class Converters
{
	public static FuncMultiValueConverter<Thickness, Thickness> ThicknessSubtractConverter { get; } =
		new(thicknesses => thicknesses.Aggregate((x, y) => x - y));
}