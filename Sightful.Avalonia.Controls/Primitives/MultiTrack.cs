using System.Collections.Immutable;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;

namespace Sightful.Avalonia.Controls.Primitives;

[PseudoClasses(":vertical", ":horizontal")]
public sealed class MultiTrack : Control
{
	internal const byte MinimumValuesCount = 2;

	public static readonly StyledProperty<decimal> RangeProperty =
		AvaloniaProperty.Register<MultiTrack, decimal>(nameof(Range), 1, coerce: CoerceRange);

	public static readonly StyledProperty<ImmutableList<decimal>> ValuesProperty =
		AvaloniaProperty.Register<MultiTrack, ImmutableList<decimal>>(nameof(Values), [0.5m, 0.5m],
			coerce: CoerceValues,
			defaultBindingMode: BindingMode.TwoWay);

	public static readonly StyledProperty<ControlTheme?> RangeButtonThemeProperty =
		AvaloniaProperty.Register<MultiTrack, ControlTheme?>(nameof(RangeButtonTheme));

	public static readonly StyledProperty<ControlTheme?> ThumbThemeProperty =
		AvaloniaProperty.Register<MultiTrack, ControlTheme?>(nameof(ThumbTheme));

	public static readonly StyledProperty<Orientation> OrientationProperty =
		AvaloniaProperty.Register<MultiTrack, Orientation>(nameof(Orientation));

	public static readonly StyledProperty<decimal> IncrementProperty =
		AvaloniaProperty.Register<MultiTrack, decimal>(nameof(Increment));

	public static readonly StyledProperty<decimal> MinimumValueProperty =
		AvaloniaProperty.Register<MultiTrack, decimal>(nameof(MinimumValue));

	static MultiTrack()
	{
		AffectsMeasure<MultiTrack>(RangeButtonThemeProperty, ThumbThemeProperty, OrientationProperty);
		AffectsArrange<MultiTrack>(ValuesProperty, RangeButtonThemeProperty, ThumbThemeProperty, OrientationProperty);
		PointerPressedEvent.AddClassHandler<MultiTrack>((track, args) => track._dragManager.OnPointerPressed(args),
			RoutingStrategies.Tunnel, true);
		PointerMovedEvent.AddClassHandler<MultiTrack>((track, args) => track._dragManager.OnPointerMoved(args),
			RoutingStrategies.Tunnel, true);
		PointerReleasedEvent.AddClassHandler<MultiTrack>((track, _) => track._dragManager.OnPointerReleased(),
			RoutingStrategies.Tunnel, true);
	}

	public Orientation Orientation
	{
		get => GetValue(OrientationProperty);
		set => SetValue(OrientationProperty, value);
	}

	public decimal Range
	{
		get => GetValue(RangeProperty);
		set => SetValue(RangeProperty, value);
	}

	public ImmutableList<decimal> Values
	{
		get => GetValue(ValuesProperty);
		set => SetValue(ValuesProperty, value);
	}

	public ControlTheme? RangeButtonTheme
	{
		get => GetValue(RangeButtonThemeProperty);
		set => SetValue(RangeButtonThemeProperty, value);
	}

	public ControlTheme? ThumbTheme
	{
		get => GetValue(ThumbThemeProperty);
		set => SetValue(ThumbThemeProperty, value);
	}

	public decimal Increment
	{
		get => GetValue(IncrementProperty);
		set => SetValue(IncrementProperty, value);
	}

	public decimal MinimumValue
	{
		get => GetValue(MinimumValueProperty);
		set => SetValue(MinimumValueProperty, value);
	}

	public MultiTrack()
	{
		_arranger = new MultiTrackArranger(Values, LogicalChildren, Range);
		_childrenManager = new MultiTrackChildrenManager(LogicalChildren, VisualChildren);
		_dragManager = new MultiTrackDragManager(this);
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		return _arranger.Measure(availableSize);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		return _arranger.Arrange(finalSize);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ValuesProperty)
			OnValuesChanged();
		else if (change.Property == RangeButtonThemeProperty)
			_childrenManager.RangeButtonTheme = RangeButtonTheme;
		else if (change.Property == ThumbThemeProperty)
			_childrenManager.ThumbTheme = ThumbTheme;
		else if (change.Property == RangeProperty)
			_arranger.Range = Range;
		else if (change.Property == OrientationProperty)
			_arranger.Orientation = Orientation;
	}

	private readonly MultiTrackArranger _arranger;
	private readonly MultiTrackChildrenManager _childrenManager;
	private readonly MultiTrackDragManager _dragManager;

	private void OnValuesChanged()
	{
		_arranger.Values = Values;
		_childrenManager.ValuesCount = (byte)Values.Count;
		foreach (var (button, value) in LogicalChildren.OfType<Button>().Zip(Values))
			button.DataContext = value;
	}

	private static decimal CoerceRange(AvaloniaObject sender, decimal value)
	{
		return value > 0 ? value : sender.GetValue(RangeProperty);
	}

	private static ImmutableList<decimal> CoerceValues(AvaloniaObject sender, ImmutableList<decimal> values)
	{
		if (values.Count < MinimumValuesCount)
		{
			var missingValuesCount = MinimumValuesCount - values.Count;
			var newValues = Enumerable.Repeat(0m, missingValuesCount);
			values = values.AddRange(newValues);
		}

		var range = sender.GetValue(RangeProperty);
		var valuesSum = values.Sum();
		if (valuesSum == 0)
			return Enumerable.Repeat(range / values.Count, values.Count).ToImmutableList();
		if (valuesSum == range)
			return values;
		var correction = range / valuesSum;
		return values.Select(value => value * correction).ToImmutableList();
	}
}