using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

namespace Sightful.Avalonia.TreeDataGrid;

internal sealed class DesignTreeDataGridViewModel
{
	public static DesignTreeDataGridViewModel Instance { get; } = new();
	
	public FlatTreeDataGridSource<User> UsersSource { get; }

	public DesignTreeDataGridViewModel()
	{
		UsersSource = new FlatTreeDataGridSource<User>([
			new User("Neil", "Armstrong"),
			new User("Buzz", "Lightyear"),
			new User("James", "Kirk"),
			new User("James", "Kirk")
		])
		{
			Columns =
			{
				new TextColumn<User, string>("First name", x => x.FirstName),
				new TextColumn<User, string>("Last name", x => x.LastName)
			}
		};
	}
}