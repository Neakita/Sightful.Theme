using Avalonia;
using Avalonia.Controls;

namespace Sightful.Avalonia.Assists;

public static class WindowAssist
{
	#region LeftContent

	public static readonly AvaloniaProperty<object?> LeftContentProperty =
		AvaloniaProperty.RegisterAttached<Window, object?>("LeftContent", typeof(WindowAssist));

	public static object? GetLeftContent(AvaloniaObject element)
	{
		return element.GetValue(LeftContentProperty);
	}

	public static void SetLeftContent(AvaloniaObject element, object? value)
	{
		element.SetValue(LeftContentProperty, value);
	}

	#endregion

	#region LeftContentWidth

	public static readonly AvaloniaProperty<double> LeftContentWidthProperty =
		AvaloniaProperty.RegisterAttached<Window, double>("LeftContentWidth", typeof(WindowAssist));

	public static double GetLeftContentWidth(AvaloniaObject element)
	{
		return element.GetValue<double>(LeftContentWidthProperty);
	}

	public static void SetLeftContentWidth(AvaloniaObject element, double value)
	{
		element.SetValue(LeftContentWidthProperty, value);
	}

	#endregion

	#region TopContent

	public static readonly AvaloniaProperty<object?> TopContentProperty =
		AvaloniaProperty.RegisterAttached<Window, object?>("TopContent", typeof(WindowAssist));

	public static object? GetTopContent(AvaloniaObject element)
	{
		return element.GetValue(TopContentProperty);
	}

	public static void SetTopContent(AvaloniaObject element, object? value)
	{
		element.SetValue(TopContentProperty, value);
	}

	#endregion

	#region OverlayContent

	public static readonly AvaloniaProperty<object?> OverlayContentProperty =
		AvaloniaProperty.RegisterAttached<Window, object?>("OverlayContent", typeof(WindowAssist));

	public static object? GetOverlayContent(AvaloniaObject element)
	{
		return element.GetValue(OverlayContentProperty);
	}

	public static void SetOverlayContent(AvaloniaObject element, object? value)
	{
		element.SetValue(OverlayContentProperty, value);
	}

	#endregion
}