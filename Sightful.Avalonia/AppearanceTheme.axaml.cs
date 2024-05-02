using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Sightful.Avalonia;

public class AppearanceTheme : ResourceDictionary
{
	private static Color BoxShadowColor => new(0x60, 0, 0, 0);
	
	public bool IsInitialized { get; private set; }
	public virtual CornerRadius ControlCornerRadius => new(12);
	public virtual TimeSpan ShrinkingDuration => TimeSpan.FromMilliseconds(500);
	public virtual TimeSpan BrushTransitionDuration => TimeSpan.FromMilliseconds(100);
	public virtual Easing ShrinkingEasing { get; } = new ElasticEaseOut();
	public virtual double Shrinking => 2;
	public virtual BoxShadows BoxShadow => new(new BoxShadow
	{
		OffsetY = 5,
		Blur = 5,
		Color = BoxShadowColor,
	});
	public virtual BoxShadows HiddenBoxShadow => new(new BoxShadow
	{
		Spread = -1, // the thin border will still be visible if it is set to 0
		Color = BoxShadowColor,
	});
	public virtual PlacementMode ToolTipPlacement => PlacementMode.Top;

	public AppearanceTheme()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public void Initialize()
	{
		if (IsInitialized)
			return;
		SetResources();
		IsInitialized = true;
	}

	private void SetResources()
	{
		SetResource(ControlCornerRadius);
		SetResource(ShrinkingDuration);
		SetResource(BrushTransitionDuration);
		SetResource(ShrinkingEasing);
		SetResource(BoxShadow);
		SetResource(HiddenBoxShadow);
		SetResource(ToolTipPlacement);
		SetResource(Shrinking);
	}

	private void SetResource<T>(T value, [CallerArgumentExpression(nameof(value))] string key = "")
	{
		this[key] = value;
	}
}