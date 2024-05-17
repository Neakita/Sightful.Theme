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
	
	#region TitleBarHeight

	public static readonly AvaloniaProperty<double> CaptionButtonsWidthProperty =
		AvaloniaProperty.RegisterAttached<Window, double>("CaptionButtonsWidth", typeof(InternalWindowAssist));

	public static double GetCaptionButtonsWidth(AvaloniaObject element)
	{
		return element.GetValue<double>(CaptionButtonsWidthProperty);
	}

	public static void SetCaptionButtonsWidth(AvaloniaObject element, double value)
	{
		element.SetValue(CaptionButtonsWidthProperty, value);
	}

	#endregion
}