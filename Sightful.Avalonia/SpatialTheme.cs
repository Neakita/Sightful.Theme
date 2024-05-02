using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;

namespace Sightful.Avalonia;

public class SpatialTheme : ResourceDictionary
{
	public bool IsInitialized { get; private set; }
	public virtual double ControlsHeight => 40;
	public virtual double WideControlsHeight => 48;
	public virtual Thickness ButtonPadding => new(16, 8);
	public virtual Thickness SquareButtonPadding => new(10);
	public virtual Thickness WideSquareButtonPadding => new(12);
	public virtual Thickness ControlsMargin => new(6);
	public virtual double FontSize => 18;

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
		AddResource(SquareButtonPadding);
		AddResource(WideSquareButtonPadding);
		AddResource(ControlsMargin);
		AddResource(FontSize);
	}

	private void AddResource<T>(T value, [CallerArgumentExpression(nameof(value))] string key = "")
	{
		Add(key, value);
	}
}