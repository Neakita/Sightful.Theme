using Avalonia;
using Avalonia.Data.Converters;

namespace Sightful.Avalonia;

internal static class Converters
{
	public static ItemToWidthConverter ItemToWidthConverter { get; } = new();
	public static ItemToPositionConverter ItemToPositionConverter { get; } = new();
	public static TwoWayFuncValueConverter<double, Rect> HeightRectConverter { get; } =
		new(height => new Rect(0, 0, 0, height), rect => rect.Height);
	public static TwoWayFuncValueConverter<double, Rect> WidthRectConverter { get; } =
		new(width => new Rect(0, 0, width, 0), rect => rect.Width);
	public static FuncValueConverter<double, Thickness> DoubleToRightMarginConverter { get; } =
		new(value => new Thickness(0, 0, value, 0));
	public static FuncMultiValueConverter<Thickness, Thickness> ThicknessMultiplicationConverter { get; } =
		new(thicknesses => thicknesses.Aggregate((x, y) => new Thickness(x.Left * y.Left, x.Top * y.Top, x.Right * y.Right, x.Bottom * y.Bottom)));
}