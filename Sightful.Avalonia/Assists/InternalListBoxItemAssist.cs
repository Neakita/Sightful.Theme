using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Sightful.Avalonia.Assists;

internal static class InternalListBoxItemAssist
{
	#region Fade

	public static readonly AvaloniaProperty<IBrush?> FadeProperty =
		AvaloniaProperty.RegisterAttached<Button, IBrush?>("Fade", typeof(InternalListBoxItemAssist));

	public static IBrush? GetFade(AvaloniaObject element)
	{
		return element.GetValue<IBrush?>(FadeProperty);
	}

	public static void SetFade(AvaloniaObject element, IBrush? value)
	{
		element.SetValue(FadeProperty, value);
	}

	#endregion
}