using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Material.Icons;
using Material.Icons.Avalonia;

namespace Sightful.Avalonia.Controls;

public sealed class MaterialIconText : TemplatedControl
{
	public static readonly StyledProperty<MaterialIconKind> KindProperty =
		MaterialIcon.KindProperty.AddOwner<MaterialIconText>();

	public static readonly StyledProperty<string?> TextProperty =
		TextBlock.TextProperty.AddOwner<MaterialIconText>();

	public static readonly StyledProperty<double> SpacingProperty =
		StackPanel.SpacingProperty.AddOwner<MaterialIconText>();

	public MaterialIconKind Kind
	{
		get => GetValue(KindProperty);
		set => SetValue(KindProperty, value);
	}

	public string? Text
	{
		get => GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public double Spacing
	{
		get => GetValue(SpacingProperty);
		set => SetValue(SpacingProperty, value);
	}
}