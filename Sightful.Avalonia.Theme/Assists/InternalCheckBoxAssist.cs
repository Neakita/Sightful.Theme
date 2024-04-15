using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Sightful.Avalonia.Theme.Assists;

public static class InternalCheckBoxAssist
{
	#region Glyph

	public static readonly AvaloniaProperty<object?> GlyphProperty =
		AvaloniaProperty.RegisterAttached<CheckBox, object?>("Glyph", typeof(InternalCheckBoxAssist));

	public static object? GetGlyph(AvaloniaObject element)
	{
		return element.GetValue(GlyphProperty);
	}

	public static void SetGlyph(AvaloniaObject element, object? value)
	{
		element.SetValue(GlyphProperty, value);
	}

	#endregion

	#region GlyphBackground

	public static readonly AvaloniaProperty<IBrush?> GlyphBackgroundProperty =
		AvaloniaProperty.RegisterAttached<CheckBox, IBrush?>("GlyphBackground", typeof(InternalCheckBoxAssist));

	public static IBrush? GetGlyphBackground(AvaloniaObject element)
	{
		return element.GetValue<IBrush?>(GlyphBackgroundProperty);
	}

	public static void SetGlyphBackground(AvaloniaObject element, IBrush? value)
	{
		element.SetValue(GlyphBackgroundProperty, value);
	}

	#endregion

	#region GlyphForeground

	public static readonly AvaloniaProperty<IBrush?> GlyphForegroundProperty =
		AvaloniaProperty.RegisterAttached<CheckBox, IBrush?>("GlyphForeground", typeof(InternalCheckBoxAssist));

	public static IBrush? GetGlyphForeground(AvaloniaObject element)
	{
		return element.GetValue<IBrush?>(GlyphForegroundProperty);
	}

	public static void SetGlyphForeground(AvaloniaObject element, IBrush? value)
	{
		element.SetValue(GlyphForegroundProperty, value);
	}

	#endregion
}