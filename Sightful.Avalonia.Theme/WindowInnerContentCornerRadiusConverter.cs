using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Sightful.Avalonia.Theme;

// It is MultiValue for possible future extension
internal sealed class WindowInnerContentCornerRadiusConverter : IMultiValueConverter
{
	public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
	{
		var cornerRadius = (CornerRadius)parameter;
		var leftContent = values[0];
		if (leftContent == null)
			cornerRadius = new CornerRadius(0, cornerRadius.TopRight, cornerRadius.BottomRight, cornerRadius.BottomLeft);
		return cornerRadius;
	}
}