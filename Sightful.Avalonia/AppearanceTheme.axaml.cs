using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia;

public class AppearanceTheme : ResourceDictionary
{
	public bool IsInitialized { get; private set; }
	public virtual CornerRadius ControlCornerRadius => new(12);
	public virtual TimeSpan ShrinkingDuration => TimeSpan.FromMilliseconds(500);
	public virtual TimeSpan BrushTransitionDuration => TimeSpan.FromMilliseconds(100);
	public virtual Easing ShrinkingEasing { get; } = new ElasticEaseOut();
	public virtual double Shrinking => 2;
	public virtual Color ShadowColor => new(0x20, 0, 0, 0);
	public virtual Color HeavyShadowColor => new(0x60, 0, 0, 0);
	public virtual double ShadowOffsetY => 5;
	public virtual double HeavyShadowOffsetY => 5;
	public virtual double ShadowBlurRadius => 5;
	public virtual double HeavyShadowBlurRadius => 10;

	// makes it possible to fully display the shadow even with ClipToBounds="True"
	public virtual Thickness ShadowContainerMargin => new(
		ShadowBlurRadius,
		ShadowOffsetY - ShadowBlurRadius,
		ShadowBlurRadius,
		ShadowOffsetY + ShadowBlurRadius);

	// makes it possible to fully display the shadow even with ClipToBounds="True"
	public virtual Thickness HeavyShadowContainerMargin => new(
		HeavyShadowBlurRadius,
		HeavyShadowOffsetY - HeavyShadowBlurRadius,
		HeavyShadowBlurRadius,
		HeavyShadowOffsetY + HeavyShadowBlurRadius);

	public virtual BoxShadows BoxShadow => new(new BoxShadow
	{
		OffsetY = ShadowOffsetY,
		Blur = ShadowBlurRadius,
		Color = ShadowColor,
	});
	public virtual BoxShadows HeavyBoxShadow => new(new BoxShadow
	{
		OffsetY = HeavyShadowOffsetY,
		Blur = HeavyShadowBlurRadius,
		Color = HeavyShadowColor,
	});
	public virtual BoxShadows HiddenBoxShadow => new(new BoxShadow
	{
		Spread = -1, // the thin border will still be visible if it is set to 0
		Color = ShadowColor,
	});
	public virtual PlacementMode ToolTipPlacement => PlacementMode.Top;
	public virtual double ToolTipVerticalOffset => ShadowContainerMargin.Bottom; // compensate shadow margin
	public virtual double HeavyToolTipVerticalOffset => HeavyShadowContainerMargin.Bottom; // compensate shadow margin

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
		SetResource(ShadowColor);
		SetResource(ShadowOffsetY);
		SetResource(ShadowBlurRadius);
		SetResource(ToolTipVerticalOffset);
		SetResource(ShadowContainerMargin);
		SetResource(HeavyShadowContainerMargin);
		SetResource(HeavyToolTipVerticalOffset);
		SetResource(HeavyBoxShadow);
	}

	private void SetResource<T>(T value, [CallerArgumentExpression(nameof(value))] string key = "")
	{
		Guard.IsTrue(ContainsKey(key));
		this[key] = value;
	}
}