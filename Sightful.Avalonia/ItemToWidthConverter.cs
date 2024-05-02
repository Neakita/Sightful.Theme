using System.Globalization;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Sightful.Avalonia;

internal sealed class ItemToWidthConverter : IMultiValueConverter
{
	public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
	{
		var tabStrip = (TabStrip?)values[0];
		if (tabStrip == null)
			return BindingValueType.UnsetValue;
		var container = tabStrip.ContainerFromIndex((int)values[1]);
		if (container == null)
			return BindingValueType.UnsetValue;
		return container.Bounds.Width;
	}
}