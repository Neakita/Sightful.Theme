using System.Collections.Immutable;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Sightful.Avalonia.Controls;

public sealed class MultiSlider : TemplatedControl
{
	public static readonly StyledProperty<ImmutableList<double>> ValuesProperty =
		MultiTrack.ValuesProperty.AddOwner<MultiSlider>();

	public ImmutableList<double> Values
	{
		get => GetValue(ValuesProperty);
		set => SetValue(ValuesProperty, value);
	}
}