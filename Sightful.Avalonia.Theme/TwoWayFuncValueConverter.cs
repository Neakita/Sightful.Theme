using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Utilities;

namespace Sightful.Avalonia.Theme;

internal sealed class TwoWayFuncValueConverter<TIn, TOut> : IValueConverter
{
	private readonly Func<TIn?, TOut> _convert;
	private readonly Func<TOut?, TIn> _convertBack;

	public TwoWayFuncValueConverter(Func<TIn?, TOut> convert, Func<TOut?, TIn> convertBack)
	{
		_convert = convert;
		_convertBack = convertBack;
	}

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (TypeUtilities.CanCast<TIn>(value))
			return _convert((TIn?)value);
		return AvaloniaProperty.UnsetValue;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (TypeUtilities.CanCast<TOut>(value))
			return _convertBack((TOut?)value);
		return AvaloniaProperty.UnsetValue;
	}
}