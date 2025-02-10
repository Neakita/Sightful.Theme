using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Sightful.Avalonia.Assists;

public static class AdornerAssist
{
	public static readonly AttachedProperty<IBrush?> AdornerBorderBrushProperty =
		AvaloniaProperty.RegisterAttached<AvaloniaObject, IBrush?>("AdornerBorderBrush", typeof(TextBoxAssist));

	public static void SetAdornerBorderBrush(TextBox element, IBrush? value) => element.SetValue(AdornerBorderBrushProperty, value);
	public static IBrush? GetAdornerBorderBrush(TextBox element) => element.GetValue(AdornerBorderBrushProperty);
}