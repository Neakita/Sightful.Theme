using System.Collections.Immutable;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia.Controls;

[PseudoClasses(":vertical", ":horizontal")]
public sealed class MultiTrack : Control
{
	public static readonly StyledProperty<double> RangeProperty =
		AvaloniaProperty.Register<MultiTrack, double>(nameof(Range), 1, coerce: CoerceRange);

	public static readonly StyledProperty<double> MinimumValueProperty =
		AvaloniaProperty.Register<MultiTrack, double>(nameof(MinimumValue));

	public static readonly StyledProperty<ImmutableList<double>> ValuesProperty =
		AvaloniaProperty.Register<MultiTrack, ImmutableList<double>>(nameof(Values), [0.5, 0.5], coerce: CoerceValues,
			defaultBindingMode: BindingMode.TwoWay);

	public static readonly StyledProperty<ControlTheme?> RangeButtonThemeProperty =
		AvaloniaProperty.Register<MultiTrack, ControlTheme?>(nameof(RangeButtonTheme));

	public static readonly StyledProperty<ControlTheme?> ThumbThemeProperty =
		AvaloniaProperty.Register<MultiTrack, ControlTheme?>(nameof(ThumbTheme));

	public static readonly StyledProperty<Orientation> OrientationProperty =
		AvaloniaProperty.Register<MultiTrack, Orientation>(nameof(Orientation));

	public Orientation Orientation
	{
		get => GetValue(OrientationProperty);
		set => SetValue(OrientationProperty, value);
	}

	static MultiTrack()
	{
		ValuesProperty.Changed.AddClassHandler<MultiTrack>((track, args) => track.ValuesChanged(args));
		RangeButtonThemeProperty.Changed.AddClassHandler<MultiTrack>((track, args) =>
			track.RangeButtonThemeChanged((AvaloniaPropertyChangedEventArgs<ControlTheme?>)args));
		ThumbThemeProperty.Changed.AddClassHandler<MultiTrack>((track, args) =>
			track.ThumbThemeChanged((AvaloniaPropertyChangedEventArgs<ControlTheme?>)args));
		AffectsMeasure<MultiTrack>(RangeButtonThemeProperty, ThumbThemeProperty, OrientationProperty);
		AffectsArrange<MultiTrack>(ValuesProperty, RangeButtonThemeProperty, ThumbThemeProperty, OrientationProperty);
	}

	public double Range
	{
		get => GetValue(RangeProperty);
		set => SetValue(RangeProperty, value);
	}

	public double MinimumValue
	{
		get => GetValue(MinimumValueProperty);
		set => SetValue(MinimumValueProperty, value);
	}

	public ImmutableList<double> Values
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
		Button initialButton = new()
		{
			Theme = RangeButtonTheme
		};
		AddChild(initialButton);
		for (var i = 1; i < Values.Count; i++)
		{
			Thumb thumb = new()
			{
				Theme = ThumbTheme
			};
			thumb.DragDelta += OnThumbDrag;
			AddChild(thumb);
			Button button = new()
			{
				Theme = RangeButtonTheme
			};
			AddChild(button);
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
		_density = availableLength - thumbsLength;

		double passedLength = 0;
		foreach (var (child, length) in LogicalChildren.Cast<Layoutable>().Zip(GetPiecesLengths()))
		{
			child.Arrange(GetArrangeRectangle(finalSize, passedLength, length));
			passedLength += length;
		}
		return Orientation == Orientation.Horizontal
			? finalSize.WithWidth(passedLength)
			: finalSize.WithHeight(passedLength);
	}

	private const int MinimumValuesCount = 2;
	private double _density;

	private static double CoerceRange(AvaloniaObject sender, double value)
	{
		return ValidateDouble(value) && value > 0 ? value : sender.GetValue(RangeProperty);
	}

	private static ImmutableList<double> CoerceValues(AvaloniaObject sender, ImmutableList<double> values)
	{
		var minimumValue = sender.GetValue(MinimumValueProperty);
		for (var i = 0; i < values.Count; i++)
		{
			var value = values[i];
			if (!ValidateDouble(value))
				values = values.SetItem(i, minimumValue);
		}

		if (values.Count < MinimumValuesCount)
		{
			var missingValuesCount = MinimumValuesCount - values.Count;
			var newValues = Enumerable.Repeat(minimumValue, missingValuesCount);
			values = values.AddRange(newValues);
		}

		var range = sender.GetValue(RangeProperty);
		var valuesSum = values.Sum();
		if (valuesSum == 0)
			return Enumerable.Repeat(range / values.Count, values.Count).ToImmutableList();
		var correction = range / valuesSum;
		return values.Select(value => value * correction).ToImmutableList();
	}

	private static bool ValidateDouble(double value)
	{
		return !double.IsInfinity(value) && !double.IsNaN(value);
	}

	private void ValuesChanged(AvaloniaPropertyChangedEventArgs args)
	{
		Guard.IsNotNull(args.OldValue);
		Guard.IsNotNull(args.NewValue);
		var oldValues = (ImmutableList<double>)args.OldValue;
		var newValues = (ImmutableList<double>)args.NewValue;
		if (newValues.Count > oldValues.Count)
			for (int i = 0; i < newValues.Count - oldValues.Count; i++)
			{
				Thumb thumb = new()
				{
					Theme = ThumbTheme
				};
				thumb.DragDelta += OnThumbDrag;
				AddChild(thumb);
				Button button = new()
				{
					Theme = RangeButtonTheme
				};
				AddChild(button);
			}
		else if (newValues.Count < oldValues.Count)
			foreach (var thumb in RemoveLastChildren((oldValues.Count - newValues.Count) * 2).OfType<Thumb>())
				thumb.DragDelta -= OnThumbDrag;
	}

	private void AddChild(Visual item)
	{
		LogicalChildren.Add(item);
		VisualChildren.Add(item);
	}

	private IReadOnlyCollection<Visual> RemoveLastChildren(int count)
	{
		Guard.IsEqualTo(LogicalChildren.Count, VisualChildren.Count);
		var index = LogicalChildren.Count - count;
		var children = VisualChildren.TakeLast(count).ToImmutableList();
		RemoveChildrenRange(index, count);
		return children;
	}

	private void RemoveChildrenRange(int index, int count)
	{
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

	private IEnumerable<double> GetPiecesLengths()
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
			else yield return GetDesiredLength((Thumb)child);
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

	private void OnThumbDrag(object? sender, VectorEventArgs args)
	{
		Guard.IsNotNull(sender);
		var thumb = (Thumb)sender;
		var thumbIndex = LogicalChildren.IndexOf(thumb);
		var valueIndex = thumbIndex / 2;
		var valueDelta = Orientation == Orientation.Horizontal ? args.Vector.X : args.Vector.Y;
		valueDelta /= _density;
		Values = Shift(Values, valueIndex, valueDelta);
		foreach (var value in Values)
			Guard.IsGreaterThanOrEqualTo(value, 0);
	}

	private ImmutableList<double> Shift(ImmutableList<double> values, int index, double delta)
	{
		Guard.IsCloseTo(values.Sum(), Range, double.Epsilon);
		var builder = values.ToBuilder();
		var oldValue = builder[index];
		var newValue = oldValue + delta;
		builder[index] = newValue;
		builder[index + 1] = builder[index + 1] - newValue + oldValue;
		Guard.IsCloseTo(builder.Sum(), Range, double.Epsilon);
		return builder.ToImmutable();
	}
}