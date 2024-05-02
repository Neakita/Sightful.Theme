using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Sightful.Avalonia.Assists;

public static class TabStripItemAssist
{
	#region HoveredBackground

	public static readonly AvaloniaProperty<IBrush?> HoveredBackgroundProperty =
		AvaloniaProperty.RegisterAttached<TabStripItem, IBrush?>("HoveredBackground", typeof(TabStripItemAssist));

	public static IBrush? GetHoveredBackground(AvaloniaObject element)
	{
		return element.GetValue<IBrush?>(HoveredBackgroundProperty);
	}

	public static void SetHoveredBackground(AvaloniaObject element, IBrush? value)
	{
		element.SetValue(HoveredBackgroundProperty, value);
	}

	#endregion

	#region PressedBackground
	
	public static readonly AvaloniaProperty<IBrush?> PressedBackgroundProperty =
		AvaloniaProperty.RegisterAttached<TabStripItem, IBrush?>("PressedBackground", typeof(TabStripItemAssist));
	
	public static IBrush? GetPressedBackground(AvaloniaObject element)
	{
		return element.GetValue<IBrush?>(PressedBackgroundProperty);
	}

	public static void SetPressedBackground(AvaloniaObject element, IBrush? value)
	{
		element.SetValue(PressedBackgroundProperty, value);
	}

	#endregion
	
	#region SelectedBackground
	
	public static readonly AvaloniaProperty<IBrush?> SelectedBackgroundProperty =
		AvaloniaProperty.RegisterAttached<TabStripItem, IBrush?>("SelectedBackground", typeof(TabStripItemAssist));
	
	public static IBrush? GetSelectedBackground(AvaloniaObject element)
	{
		return element.GetValue<IBrush?>(SelectedBackgroundProperty);
	}

	public static void SetSelectedBackground(AvaloniaObject element, IBrush? value)
	{
		element.SetValue(SelectedBackgroundProperty, value);
	}

	#endregion
}