using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Sightful.Avalonia.Theme.Assists;

public static class ShadowAssist
{
	public static readonly AvaloniaProperty<BoxShadows> BoxShadowProperty =
		AvaloniaProperty.RegisterAttached<Control, BoxShadows>("BoxShadow", typeof(ShadowAssist));
	
	public static BoxShadows GetBoxShadow(AvaloniaObject element)
	{
		return element.GetValue<BoxShadows>(BoxShadowProperty);
	}

	public static void SetBoxShadow(AvaloniaObject element, BoxShadows value)
	{
		element.SetValue(BoxShadowProperty, value);
	}
}