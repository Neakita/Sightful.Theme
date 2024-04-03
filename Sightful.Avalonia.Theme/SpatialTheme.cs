using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;

namespace Sightful.Avalonia.Theme;

public class SpatialTheme : ResourceDictionary
{
	public bool IsInitialized { get; private set; }
	public virtual double ControlsHeight => 40;
	public virtual double WideControlsHeight => 48;
	public virtual Thickness ButtonPadding => new(16, 8);

	public void Initialize()
	{
		if (IsInitialized)
			return;
		AddResources();
		IsInitialized = true;
	}

	private void AddResources()
	{
		AddResource(ControlsHeight);
		AddResource(WideControlsHeight);
		AddResource(ButtonPadding);
	}

	private void AddResource<T>(T value, [CallerArgumentExpression(nameof(value))] string key = "")
	{
		Add(key, value);
	}
}