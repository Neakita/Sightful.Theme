using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Input;

namespace Sightful.Avalonia.Controls.GestureBox.Converters;

internal sealed class IsKeyGestureConverter : IValueConverter
{
	public static IsKeyGestureConverter Instance { get; } = new();

	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is KeyGesture;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}