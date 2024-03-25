using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Avalonia.Themes.Sightful;

public sealed class DefaultTheme : Styles
{
	public DefaultTheme(IServiceProvider? serviceProvider = null)
	{
		AvaloniaXamlLoader.Load(serviceProvider, this);
	}
}