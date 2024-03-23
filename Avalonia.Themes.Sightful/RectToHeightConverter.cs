using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Avalonia.Themes.Sightful;

internal sealed class RectToHeightConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		Rect rect = (Rect)value;
		return rect.Height;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}