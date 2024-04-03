using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;

namespace Sightful.Avalonia.Theme;

public class ThemeAppearance : ResourceDictionary
{
	public bool IsInitialized { get; private set; }
	public virtual CornerRadius ControlCornerRadius => new(12);
	public virtual TimeSpan ShrinkingDuration => TimeSpan.FromMilliseconds(500);
	public virtual TimeSpan BrushTransitionDuration => TimeSpan.FromMilliseconds(100);
	public virtual Easing ShrinkingEasing { get; } = new ElasticEaseOut();

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
		AddResource(ShrinkingDuration);
		AddResource(BrushTransitionDuration);
		AddResource(ShrinkingEasing);
	}

	private void AddResource<T>(T value, [CallerArgumentExpression(nameof(value))] string key = "")
	{
		Add(key, value);
	}
}