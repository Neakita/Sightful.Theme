using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia;

internal sealed class ComboBoxCornerRadiusConverter : IMultiValueConverter
{
	public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
	{
		Guard.IsEqualTo(values.Count, 4);
		if (values[0] is not CornerRadius cornerRadius ||
		    values[1] is not bool isPopupOpen ||
		    values[2] is not PlacementMode placement ||
		    values[3] is not bool invert)
			return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
		if (!isPopupOpen)
			return cornerRadius;
		if (invert)
			placement = placement switch
			{
				PlacementMode.Top => PlacementMode.Bottom,
				PlacementMode.Bottom => PlacementMode.Top,
				_ => placement
			};
		return placement switch
		{
			PlacementMode.Top => new CornerRadius(0, 0, cornerRadius.BottomRight, cornerRadius.BottomLeft),
			PlacementMode.Bottom => new CornerRadius(cornerRadius.TopLeft, cornerRadius.TopRight, 0, 0),
			_ => cornerRadius
		};
	}
}