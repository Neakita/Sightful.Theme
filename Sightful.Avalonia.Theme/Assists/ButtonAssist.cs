using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Avalonia.Themes.Sightful.Assists;

public static class ButtonAssist
{
	public static readonly AvaloniaProperty<BoxShadows> BoxShadowProperty =
		AvaloniaProperty.RegisterAttached<Button, BoxShadows>("BoxShadow", typeof(ButtonAssist));
	
	/*public static readonly AvaloniaProperty<IBrush?> HoverBackgroundProperty =
		AvaloniaProperty.RegisterAttached<Button, IBrush?>("HoverBackground", typeof(ButtonAssist));
	
	public static readonly AvaloniaProperty<IBrush?> PressedBackgroundProperty =
		AvaloniaProperty.RegisterAttached<Button, IBrush?>("PressedBackground", typeof(ButtonAssist));*/
}