using System.Collections.Immutable;
using Avalonia;
using Avalonia.Controls.Primitives;
using Sightful.Avalonia.Controls.Primitives;

namespace Sightful.Avalonia.Controls;

public sealed class MultiSlider : TemplatedControl
{
	public static readonly StyledProperty<ImmutableList<decimal>> ValuesProperty =
		MultiTrack.ValuesProperty.AddOwner<MultiSlider>();

	public ImmutableList<decimal> Values
	{
		get => GetValue(ValuesProperty);
		set => SetValue(ValuesProperty, value);
	}
}