using System.Globalization;
using Avalonia.Data.Converters;

namespace Sightful.Avalonia.Controls.GestureBox.Converters;

internal sealed class IsMouseButtonGestureConverter : IValueConverter
{
	public static IsMouseButtonGestureConverter Instance { get; } = new();

	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is MouseButtonGesture;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}