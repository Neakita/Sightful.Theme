using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Sightful.Avalonia.TreeDataGrid.Theme;

public sealed class SightfulTreeDataGridTheme : Styles
{
	public SightfulTreeDataGridTheme(IServiceProvider? serviceProvider = null)
	{
		AvaloniaXamlLoader.Load(serviceProvider, this);
	}
}