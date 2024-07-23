using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Sightful.Avalonia.Assists;

internal static class ToggleSwitchAssist
{
	public static readonly AttachedProperty<Brush?> KnobFillProperty =
		AvaloniaProperty.RegisterAttached<ToggleSwitch, Brush?>("KnobFill", typeof(ToggleSwitchAssist));

	public static Brush? GetKnobFill(AvaloniaObject element)
	{
		return element.GetValue(KnobFillProperty);
	}

	public static void SetKnobFill(AvaloniaObject element, Brush? brush)
	{
		element.SetValue(KnobFillProperty, brush);
	}
}