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

namespace Sightful.Avalonia.Controls;

[PseudoClasses(":vertical", ":horizontal")]
public sealed class MultiTrack : Control
{
	public static readonly StyledProperty<decimal> RangeProperty =
		AvaloniaProperty.Register<MultiTrack, decimal>(nameof(Range), 1, coerce: CoerceRange);

	public static readonly StyledProperty<ImmutableList<decimal>> ValuesProperty =
		AvaloniaProperty.Register<MultiTrack, ImmutableList<decimal>>(nameof(Values), [0.5m, 0.5m], coerce: CoerceValues,
			defaultBindingMode: BindingMode.TwoWay);

	public static readonly StyledProperty<ControlTheme?> RangeButtonThemeProperty =
		AvaloniaProperty.Register<MultiTrack, ControlTheme?>(nameof(RangeButtonTheme));

	public static readonly StyledProperty<ControlTheme?> ThumbThemeProperty =
		AvaloniaProperty.Register<MultiTrack, ControlTheme?>(nameof(ThumbTheme));

	public static readonly StyledProperty<Orientation> OrientationProperty =
		AvaloniaProperty.Register<MultiTrack, Orientation>(nameof(Orientation));

	static MultiTrack()
	{
		ValuesProperty.Changed.AddClassHandler<MultiTrack>((track, args) => track.ValuesChanged(args));
		RangeButtonThemeProperty.Changed.AddClassHandler<MultiTrack>((track, args) =>
			track.RangeButtonThemeChanged((AvaloniaPropertyChangedEventArgs<ControlTheme?>)args));
		ThumbThemeProperty.Changed.AddClassHandler<MultiTrack>((track, args) =>
			track.ThumbThemeChanged((AvaloniaPropertyChangedEventArgs<ControlTheme?>)args));
		AffectsMeasure<MultiTrack>(RangeButtonThemeProperty, ThumbThemeProperty, OrientationProperty);
		AffectsArrange<MultiTrack>(ValuesProperty, RangeButtonThemeProperty, ThumbThemeProperty, OrientationProperty);
		PointerPressedEvent.AddClassHandler<MultiTrack>((track, args) => track.OnTunnelPointerPressed(args), RoutingStrategies.Tunnel, true);
		PointerMovedEvent.AddClassHandler<MultiTrack>((track, args) => track.OnTunnelPointerMoved(args), RoutingStrategies.Tunnel, true);
		PointerReleasedEvent.AddClassHandler<MultiTrack>((track, _) => track._dragIndex = null, RoutingStrategies.Tunnel, true);
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

	public MultiTrack()
	{
		CreateRangeButton();
		for (var i = 1; i < Values.Count; i++)
		{
			CreateThumb();
			CreateRangeButton();
		}
	}

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

	private const int MinimumValuesCount = 2;
	private decimal _density;

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

	private void ValuesChanged(AvaloniaPropertyChangedEventArgs args)
	{
		Guard.IsNotNull(args.OldValue);
		Guard.IsNotNull(args.NewValue);
		var oldValues = (ImmutableList<decimal>)args.OldValue;
		var newValues = (ImmutableList<decimal>)args.NewValue;
		if (newValues.Count > oldValues.Count)
			for (int i = 0; i < newValues.Count - oldValues.Count; i++)
			{
				CreateThumb();
				CreateRangeButton();
			}
		else if (newValues.Count < oldValues.Count)
			RemoveLastChildren((oldValues.Count - newValues.Count) * 2);

		foreach (var (button, value) in LogicalChildren.OfType<Button>().Zip(Values))
			button.DataContext = value;
	}

	private void AddChild(Visual item)
	{
		LogicalChildren.Add(item);
		VisualChildren.Add(item);
	}

	private void RemoveLastChildren(int count)
	{
		Guard.IsEqualTo(LogicalChildren.Count, VisualChildren.Count);
		var index = LogicalChildren.Count - count;
		LogicalChildren.RemoveRange(index, count);
		VisualChildren.RemoveRange(index, count);
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

	private void RangeButtonThemeChanged(AvaloniaPropertyChangedEventArgs<ControlTheme?> args)
	{
		foreach (var button in LogicalChildren.OfType<Button>())
			button.Theme = args.NewValue.Value;
	}

	private void ThumbThemeChanged(AvaloniaPropertyChangedEventArgs<ControlTheme?> args)
	{
		foreach (var thumb in LogicalChildren.OfType<Thumb>())
			thumb.Theme = args.NewValue.Value;
	}

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

	private byte? _dragIndex;
	private Point _previousPosition;

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
		_previousPosition = position;

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
		var delta = position - _previousPosition;
		var distance = Orientation == Orientation.Horizontal ? delta.X : delta.Y;
		Drag(_dragIndex.Value, (decimal)distance);
		_previousPosition = position;
	}

	private void CreateRangeButton()
	{
		Button button = new();
		button.Theme = RangeButtonTheme;
		AddChild(button);
	}

	private void CreateThumb()
	{
		Thumb thumb = new();
		thumb.Theme = ThumbTheme;
		AddChild(thumb);
	}
}