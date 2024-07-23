using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Sightful.Avalonia.Assists;

internal static class ToggleSwitchAssist
{
	public static readonly AttachedProperty<Brush?> KnobFillProperty =
		AvaloniaProperty.RegisterAttached<ToggleSwitch, Brush?>("KnobFill", typeof(ToggleSwitchAssist));

	public static Brush? GetKnobFill(ToggleSwitch element)
	{
		return element.GetValue(KnobFillProperty);
	}

	public static void SetKnobFill(ToggleSwitch element, Brush? brush)
	{
		element.SetValue(KnobFillProperty, brush);
	}

	public static readonly AttachedProperty<Brush?> HoveredKnobFillProperty =
		AvaloniaProperty.RegisterAttached<ToggleSwitch, Brush?>("HoveredKnobFill", typeof(ToggleSwitchAssist));

	public static void SetHoveredKnobFill(ToggleSwitch element, Brush? value)
	{
		element.SetValue(HoveredKnobFillProperty, value);
	}

	public static Brush? GetHoveredKnobFill(ToggleSwitch element)
	{
		return element.GetValue(HoveredKnobFillProperty);
	}

	public static readonly AttachedProperty<Brush?> HoveredBackgroundProperty =
		AvaloniaProperty.RegisterAttached<ToggleSwitch, Brush?>("HoveredBackground", typeof(ToggleSwitchAssist));

	public static void SetHoveredBackground(ToggleSwitch element, Brush? value)
	{
		element.SetValue(HoveredBackgroundProperty, value);
	}

	public static Brush? GetHoveredBackground(ToggleSwitch element)
	{
		return element.GetValue(HoveredBackgroundProperty);
	}

	public static readonly AttachedProperty<Brush?> PressedBackgroundProperty =
		AvaloniaProperty.RegisterAttached<ToggleSwitch, Brush?>("PressedBackground", typeof(ToggleSwitchAssist));

	public static void SetPressedBackground(ToggleSwitch element, Brush? value)
	{
		element.SetValue(PressedBackgroundProperty, value);
	}

	public static Brush? GetPressedBackground(ToggleSwitch element)
	{
		return element.GetValue(PressedBackgroundProperty);
	}

	public static readonly AttachedProperty<Brush?> PressedKnobFillProperty =
		AvaloniaProperty.RegisterAttached<ToggleSwitch, Brush?>("PressedKnobFill", typeof(ToggleSwitchAssist));

	public static void SetPressedKnobFill(ToggleSwitch element, Brush? value)
	{
		element.SetValue(PressedKnobFillProperty, value);
	}

	public static Brush? GetPressedKnobFill(ToggleSwitch element)
	{
		return element.GetValue(PressedKnobFillProperty);
	}

	public static readonly AttachedProperty<double> ShrinkingProperty =
		AvaloniaProperty.RegisterAttached<ToggleSwitch, double>("Shrinking", typeof(ToggleSwitchAssist));

	public static void SetShrinking(ToggleSwitch element, double value)
	{
		element.SetValue(ShrinkingProperty, value);
	}

	public static double GetShrinking(ToggleSwitch element)
	{
		return element.GetValue(ShrinkingProperty);
	}
}