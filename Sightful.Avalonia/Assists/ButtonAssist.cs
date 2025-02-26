using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Sightful.Avalonia.Assists;

public static class ButtonAssist
{
	#region HoveredBackground

	public static readonly AvaloniaProperty<IBrush?> HoveredBackgroundProperty =
		AvaloniaProperty.RegisterAttached<Button, IBrush?>("HoveredBackground", typeof(ButtonAssist));

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
		AvaloniaProperty.RegisterAttached<Button, IBrush?>("PressedBackground", typeof(ButtonAssist));
	
	public static IBrush? GetPressedBackground(AvaloniaObject element)
	{
		return element.GetValue<IBrush?>(PressedBackgroundProperty);
	}

	public static void SetPressedBackground(AvaloniaObject element, IBrush? value)
	{
		element.SetValue(PressedBackgroundProperty, value);
	}

	#endregion

	public static readonly AttachedProperty<string> ThemeIdProperty =
		AvaloniaProperty.RegisterAttached<Button, string>("ThemeId", typeof(ButtonAssist));

	public static void SetThemeId(Button element, string value) => element.SetValue(ThemeIdProperty, value);
	public static string GetThemeId(Button element) => element.GetValue(ThemeIdProperty);
}