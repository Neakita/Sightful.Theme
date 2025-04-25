using Avalonia;

namespace Sightful.Avalonia.Assists;

public static class CardButtonAssist
{
	public static readonly AttachedProperty<object?> OverlayContentProperty =
		AvaloniaProperty.RegisterAttached<AvaloniaObject, object?>("OverlayContent", typeof(CardButtonAssist));

	public static void SetOverlayContent(AvaloniaObject element, object? value) => element.SetValue(OverlayContentProperty, value);
	public static object? GetOverlayContent(AvaloniaObject element) => element.GetValue(OverlayContentProperty);
}