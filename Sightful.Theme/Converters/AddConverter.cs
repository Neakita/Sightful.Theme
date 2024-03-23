using System.Globalization;
using Avalonia.Data.Converters;

namespace Sightful.Theme.Converters;

internal sealed class AddConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value switch
		{
			int i => i + (int)parameter,
			double d => d + double.Parse((string)parameter)
		};
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}