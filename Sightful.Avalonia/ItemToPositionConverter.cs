using System.Globalization;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia;

internal sealed class ItemToPositionConverter : IMultiValueConverter
{
	public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
	{
		if (values[0] is not TabStrip { IsLoaded: true } tabStrip)
			return null;
		var itemIndex = values[1];
		Guard.IsNotNull(itemIndex);
		var container = tabStrip.ContainerFromIndex((int)itemIndex);
		Guard.IsNotNull(container);
		return container.Bounds.X;
	}
}