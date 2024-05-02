using Avalonia;
using Avalonia.Controls;

namespace Sightful.Avalonia.Assists;

internal static class InternalWindowAssist
{
	#region TitleBarHeight

	

	public static readonly AvaloniaProperty<double> TitleBarHeightProperty =
		AvaloniaProperty.RegisterAttached<Window, double>("TitleBarHeight", typeof(InternalWindowAssist));

	public static double GetTitleBarHeight(AvaloniaObject element)
	{
		return element.GetValue<double>(TitleBarHeightProperty);
	}

	public static void SetTitleBarHeight(AvaloniaObject element, double value)
	{
		element.SetValue(TitleBarHeightProperty, value);
	}

	#endregion
}