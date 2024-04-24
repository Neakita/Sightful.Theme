using Avalonia;
using Avalonia.Data.Converters;

namespace Sightful.Avalonia.Theme;

internal static class Converters
{
	public static ItemToWidthConverter ItemToWidthConverter { get; } = new();
	public static ItemToPositionConverter ItemToPositionConverter { get; } = new();
	public static TwoWayFuncValueConverter<double, Rect> HeightRectConverter { get; } =
		new(height => new Rect(0, 0, 0, height), rect => rect.Height);
	public static FuncMultiValueConverter<Thickness, Thickness> ThicknessMultiplicationConverter { get; } =
		new(thicknesses => thicknesses.Aggregate((x, y) => new Thickness(x.Left * y.Left, x.Top * y.Top, x.Right * y.Right, x.Bottom * y.Bottom)));
}