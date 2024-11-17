using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Sightful.Avalonia.Assists;

public static class TextBoxAssist
{
	public static readonly AvaloniaProperty<IBrush?> WatermarkBrushProperty =
		AvaloniaProperty.RegisterAttached<TextBox, IBrush?>("WatermarkBrush", typeof(TextBoxAssist));

	public static readonly AvaloniaProperty<Thickness> LeftInnerContentMarginProperty =
		AvaloniaProperty.RegisterAttached<TextBox, Thickness>("LeftInnerContentMargin", typeof(TextBoxAssist));

	public static readonly AvaloniaProperty<Thickness> RightInnerContentMarginProperty =
		AvaloniaProperty.RegisterAttached<TextBox, Thickness>("RightInnerContentMargin", typeof(TextBoxAssist));

	public static readonly AttachedProperty<IBrush?> AdornerBorderBrushProperty =
		AvaloniaProperty.RegisterAttached<TextBox, IBrush?>("AdornerBorderBrush", typeof(TextBoxAssist));

	public static void SetAdornerBorderBrush(TextBox element, IBrush? value) => element.SetValue(AdornerBorderBrushProperty, value);
	public static IBrush? GetAdornerBorderBrush(TextBox element) => element.GetValue(AdornerBorderBrushProperty);
}