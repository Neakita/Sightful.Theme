using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Sightful.Avalonia.Assists;

internal static class ToggleSwitchAssist
{
	public static readonly AttachedProperty<IBrush?> KnobFillProperty =
		AvaloniaProperty.RegisterAttached<ToggleSwitch, IBrush?>("KnobFill", typeof(ToggleSwitchAssist));

	public static IBrush? GetKnobFill(ToggleSwitch element)
	{
		return element.GetValue(KnobFillProperty);
	}

	public static void SetKnobFill(ToggleSwitch element, IBrush? brush)
	{
		element.SetValue(KnobFillProperty, brush);
	}

	public static readonly AttachedProperty<IBrush?> HoveredKnobFillProperty =
		AvaloniaProperty.RegisterAttached<ToggleSwitch, IBrush?>("HoveredKnobFill", typeof(ToggleSwitchAssist));

	public static void SetHoveredKnobFill(ToggleSwitch element, IBrush? value)
	{
		element.SetValue(HoveredKnobFillProperty, value);
	}

	public static IBrush? GetHoveredKnobFill(ToggleSwitch element)
	{
		return element.GetValue(HoveredKnobFillProperty);
	}

	public static readonly AttachedProperty<IBrush?> HoveredBackgroundProperty =
		AvaloniaProperty.RegisterAttached<ToggleSwitch, IBrush?>("HoveredBackground", typeof(ToggleSwitchAssist));

	public static void SetHoveredBackground(ToggleSwitch element, IBrush? value)
	{
		element.SetValue(HoveredBackgroundProperty, value);
	}

	public static IBrush? GetHoveredBackground(ToggleSwitch element)
	{
		return element.GetValue(HoveredBackgroundProperty);
	}

	public static readonly AttachedProperty<IBrush?> PressedBackgroundProperty =
		AvaloniaProperty.RegisterAttached<ToggleSwitch, IBrush?>("PressedBackground", typeof(ToggleSwitchAssist));

	public static void SetPressedBackground(ToggleSwitch element, IBrush? value)
	{
		element.SetValue(PressedBackgroundProperty, value);
	}

	public static IBrush? GetPressedBackground(ToggleSwitch element)
	{
		return element.GetValue(PressedBackgroundProperty);
	}

	public static readonly AttachedProperty<IBrush?> PressedKnobFillProperty =
		AvaloniaProperty.RegisterAttached<ToggleSwitch, IBrush?>("PressedKnobFill", typeof(ToggleSwitchAssist));

	public static void SetPressedKnobFill(ToggleSwitch element, IBrush? value)
	{
		element.SetValue(PressedKnobFillProperty, value);
	}

	public static IBrush? GetPressedKnobFill(ToggleSwitch element)
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