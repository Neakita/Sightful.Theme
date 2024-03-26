using Avalonia;
using Avalonia.Controls;

namespace Sightful.Avalonia.Theme.Assists;

internal static class InternalButtonAssist
{
	#region ShrinkingThickness

	public static readonly AvaloniaProperty<Thickness> ShrinkingThicknessProperty =
		AvaloniaProperty.RegisterAttached<Button, Thickness>("ShrinkingThickness", typeof(InternalButtonAssist));

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