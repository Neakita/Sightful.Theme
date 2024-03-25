using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Sightful.Avalonia.Theme;

public sealed class DefaultTheme : Styles
{
	public DefaultTheme(IServiceProvider? serviceProvider = null)
	{
		AvaloniaXamlLoader.Load(serviceProvider, this);
	}
}