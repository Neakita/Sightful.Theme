using Avalonia;
using Avalonia.Data.Converters;

namespace Sightful.Avalonia.Theme;

internal static class Converters
{
	public static FuncMultiValueConverter<Thickness, Thickness> ThicknessSubtractConverter { get; } =
		new(thicknesses => thicknesses.Aggregate((x, y) => x - y));
	public static TwoWayFuncValueConverter<double, Rect> HeightRectConverter { get; } =
		new(height => new Rect(0, 0, 0, height), rect => rect.Height);
}