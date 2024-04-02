using Avalonia;
using Avalonia.Controls;

namespace Sightful.Avalonia.Theme.Assists;

internal static class InternalShrinkingAssist
{
	#region ShrinkingThickness

	public static readonly AvaloniaProperty<Thickness> ShrinkingThicknessProperty =
		AvaloniaProperty.RegisterAttached<Control, Thickness>("ShrinkingThickness", typeof(InternalShrinkingAssist));

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