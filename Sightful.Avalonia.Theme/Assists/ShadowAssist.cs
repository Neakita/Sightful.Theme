using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Sightful.Avalonia.Theme.Assists;

public static class ShadowAssist
{
	public static readonly AvaloniaProperty<BoxShadows> BoxShadowProperty =
		AvaloniaProperty.RegisterAttached<Button, BoxShadows>("BoxShadow", typeof(ShadowAssist));
}