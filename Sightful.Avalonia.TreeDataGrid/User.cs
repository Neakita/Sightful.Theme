namespace Sightful.Avalonia.TreeDataGrid;

internal sealed class User
{
	public string FirstName { get; }
	public string LastName { get; }

	public User(string firstName, string lastName)
	{
		FirstName = firstName;
		LastName = lastName;
	}
}