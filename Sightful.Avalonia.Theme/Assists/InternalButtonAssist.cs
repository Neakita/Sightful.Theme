using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Sightful.Avalonia.Theme.Assists;

internal static class InternalButtonAssist
{
	#region HoveredBackground

	public static readonly AvaloniaProperty<IBrush?> HoveredBackgroundProperty =
		AvaloniaProperty.RegisterAttached<Button, IBrush?>("HoveredBackground", typeof(InternalButtonAssist));

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
		AvaloniaProperty.RegisterAttached<Button, IBrush?>("PressedBackground", typeof(InternalButtonAssist));
	
	public static IBrush? GetPressedBackground(AvaloniaObject element)
	{
		return element.GetValue<IBrush?>(PressedBackgroundProperty);
	}

	public static void SetPressedBackground(AvaloniaObject element, IBrush? value)
	{
		element.SetValue(PressedBackgroundProperty, value);
	}

	#endregion

	#region ShrinkingThickness

	public static readonly AvaloniaProperty<Thickness> ShrinkingThicknessProperty =
		AvaloniaProperty.RegisterAttached<Button, Thickness>("ShrinkingThickness", typeof(InternalButtonAssist));

	public static Thickness GetShrinkingThickness(AvaloniaObject element)
	{
		return element.GetValue<Thickness>(ShrinkingThicknessProperty);
	}

	public static void SetShrinkingThickness(AvaloniaObject element, Thickness value)
	{
		element.SetValue(ShrinkingThicknessProperty, value);
	}

	#endregion
}