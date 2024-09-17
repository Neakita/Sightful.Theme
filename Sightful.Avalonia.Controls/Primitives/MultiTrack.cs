using System.Collections.Immutable;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia.Controls.Primitives;

[PseudoClasses(":vertical", ":horizontal")]
public sealed class MultiTrack : Control
{
	internal const byte MinimumValuesCount = 2;

	#region StyledProperties

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
		var correction = range / valuesSum;
		return values.Select(value => value * correction).ToImmutableList();
	}

	#endregion

	static MultiTrack()
	{
		AffectsMeasure<MultiTrack>(RangeButtonThemeProperty, ThumbThemeProperty, OrientationProperty);
		AffectsArrange<MultiTrack>(ValuesProperty, RangeButtonThemeProperty, ThumbThemeProperty, OrientationProperty);
		PointerPressedEvent.AddClassHandler<MultiTrack>((track, args) => track.OnTunnelPointerPressed(args),
			RoutingStrategies.Tunnel, true);
		PointerMovedEvent.AddClassHandler<MultiTrack>((track, args) => track.OnTunnelPointerMoved(args),
			RoutingStrategies.Tunnel, true);
		PointerReleasedEvent.AddClassHandler<MultiTrack>((track, _) => track._dragIndex = null,
			RoutingStrategies.Tunnel, true);
	}

	#region Properties

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

	#endregion

	public MultiTrack()
	{
		_childrenManager = new MultiTrackChildrenManager(LogicalChildren, VisualChildren);
	}

	#region Measure & Arrangement

	protected override Size MeasureOverride(Size availableSize)
	{
		Size desiredSize = new();
		foreach (var thumb in LogicalChildren.OfType<Thumb>())
		{
			thumb.Measure(availableSize);
			desiredSize = StackSizes(desiredSize, thumb.DesiredSize);
			availableSize = SubtractStackedSize(availableSize, thumb.DesiredSize);
		}

		return desiredSize;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		var availableLength = Orientation == Orientation.Horizontal ? finalSize.Width : finalSize.Height;
		var thumbsLength = LogicalChildren.OfType<Thumb>().Select(GetDesiredLength).Sum();
		_density = (decimal)(availableLength - thumbsLength);
		double passedLength = 0;
		foreach (var (child, length) in LogicalChildren.Cast<Layoutable>().Zip(GetPiecesLengths()))
		{
			child.Arrange(GetArrangeRectangle(finalSize, passedLength, (double)length));
			passedLength += (double)length;
		}

		return Orientation == Orientation.Horizontal
			? finalSize.WithWidth(passedLength)
			: finalSize.WithHeight(passedLength);
	}

	private Size StackSizes(Size first, Size second)
	{
		return Orientation == Orientation.Horizontal
			? new Size(first.Width + second.Width, Math.Max(first.Height, second.Height))
			: new Size(Math.Max(first.Width, second.Width), first.Height + second.Height);
	}

	private Size SubtractStackedSize(Size minuend, Size subtrahend)
	{
		return Orientation == Orientation.Horizontal
			? new Size(minuend.Width - subtrahend.Width, minuend.Height)
			: new Size(minuend.Width, minuend.Height - subtrahend.Height);
	}

	private IEnumerable<decimal> GetPiecesLengths()
	{
		for (var i = 0; i < LogicalChildren.Count; i++)
		{
			var child = LogicalChildren[i];
			if (i % 2 == 0)
			{
				Guard.IsOfType<Button>(child);
				var value = Values[i / 2];
				var normalizedValue = value / Range;
				yield return _density * normalizedValue;
			}
			else yield return (decimal)GetDesiredLength((Thumb)child);
		}
	}

	private double GetDesiredLength(Layoutable layoutable)
	{
		return Orientation == Orientation.Horizontal ? layoutable.DesiredSize.Width : layoutable.DesiredSize.Height;
	}

	private Rect GetArrangeRectangle(Size finalSize, double passedLength, double length)
	{
		return Orientation == Orientation.Horizontal
			? new Rect(passedLength, 0, length, finalSize.Height)
			: new Rect(0, passedLength, finalSize.Width, length);
	}

	#endregion

	#region Drag

	private byte? _dragIndex;
	private Point _previousDragPosition;

	private void Drag(int index, decimal distance)
	{
		var valueDelta = distance / _density;
		valueDelta = Math.Clamp(valueDelta, -Values[index], Values[index + 1]);
		Values = Shift(Values, index, valueDelta);
		foreach (var value in Values)
			Guard.IsGreaterThanOrEqualTo(value, 0);
	}

	private ImmutableList<decimal> Shift(ImmutableList<decimal> values, int index, decimal delta)
	{
		Guard.IsEqualTo(values.Sum(), Range);
		var builder = values.ToBuilder();
		builder[index] += delta;
		builder[index + 1] -= delta;
		Guard.IsEqualTo(builder.Sum(), Range);
		return builder.ToImmutable();
	}

	private void OnTunnelPointerPressed(PointerPressedEventArgs args)
	{
		var position = args.GetPosition(this);
		Point normalizedPosition = new(position.X / Bounds.Width, position.Y / Bounds.Height);
		var normalizedLength = Orientation == Orientation.Horizontal ? normalizedPosition.X : normalizedPosition.Y;
		var length = (decimal)normalizedLength * Range;
		_dragIndex = (byte?)LogicalChildren
			.OfType<Thumb>()
			.Select((thumb, index) => (thumb, index))
			.MinBy(tuple =>
			{
				var thumbPosition = args.GetPosition(tuple.thumb);
				var distance = Orientation == Orientation.Horizontal ? thumbPosition.X : thumbPosition.Y;
				return Math.Abs(distance);
			}).index;
		Guard.IsNotNull(_dragIndex);
		_previousDragPosition = position;

		decimal accumulated = 0;
		for (var i = 0; i < _dragIndex; i++)
		{
			var value = Values[i];
			accumulated += value;
		}

		var newValue = length - accumulated;
		Values = Shift(Values, _dragIndex.Value, newValue - Values[_dragIndex.Value]);
	}

	private void OnTunnelPointerMoved(PointerEventArgs args)
	{
		if (_dragIndex == null)
			return;
		var position = args.GetPosition(this);
		var delta = position - _previousDragPosition;
		var distance = Orientation == Orientation.Horizontal ? delta.X : delta.Y;
		Drag(_dragIndex.Value, (decimal)distance);
		_previousDragPosition = position;
	}

	#endregion

	private readonly MultiTrackChildrenManager _childrenManager;
	private decimal _density;

	private void OnValuesChanged()
	{
		_childrenManager.ValuesCount = (byte)Values.Count;
		foreach (var (button, value) in LogicalChildren.OfType<Button>().Zip(Values))
			button.DataContext = value;
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
	}
}