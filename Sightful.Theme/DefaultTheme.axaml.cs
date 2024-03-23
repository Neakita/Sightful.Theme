using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Sightful.Theme;

public sealed class DefaultTheme : Styles
{
	public DefaultTheme(IServiceProvider? serviceProvider = null)
	{
		AvaloniaXamlLoader.Load(serviceProvider, this);
	}
}