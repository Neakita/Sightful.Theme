using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;

namespace Sightful.Avalonia.Theme;

public class ThemeAppearance : ResourceDictionary
{
	public bool IsInitialized { get; private set; }
	public virtual CornerRadius ControlCornerRadius => new(12);

	public void Initialize()
	{
		if (IsInitialized)
			return;
		AddResources();
		IsInitialized = true;
	}

	private void AddResources()
	{
		AddResource(ControlCornerRadius);
	}

	private void AddResource<T>(T value, [CallerArgumentExpression(nameof(value))] string key = "")
	{
		Add(key, value);
	}
}