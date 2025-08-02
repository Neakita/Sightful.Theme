using System.Collections.Immutable;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Sightful.Avalonia.Controls;

internal sealed class MultiSliderValuesToObjectsCollectionConverter : IValueConverter
{
	public static MultiSliderValuesToObjectsCollectionConverter Instance { get; } = new();

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is ImmutableList<decimal> decimals)
			return decimals.Cast<object?>().ToList();
		return AvaloniaProperty.UnsetValue;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}