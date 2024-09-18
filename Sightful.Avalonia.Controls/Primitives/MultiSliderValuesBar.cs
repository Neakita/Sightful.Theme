using System.Collections.Immutable;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Styling;

namespace Sightful.Avalonia.Controls.Primitives;

[PseudoClasses(":vertical", ":horizontal")]
public sealed class MultiSliderValuesBar : Control
{
	public static readonly StyledProperty<ImmutableList<decimal>> ValuesProperty =
			MultiTrack.ValuesProperty.AddOwner<MultiSliderValuesBar>(new StyledPropertyMetadata<ImmutableList<decimal>>(defaultBindingMode: BindingMode.OneWay));

	public static readonly StyledProperty<ControlTheme?> TextBlockThemeProperty =
		AvaloniaProperty.Register<MultiSliderValuesBar, ControlTheme?>(nameof(TextBlockTheme));

	public static readonly StyledProperty<string?> StringFormatProperty =
		AvaloniaProperty.Register<MultiSliderValuesBar, string?>(nameof(StringFormat));

	public static readonly StyledProperty<Orientation> OrientationProperty =
		AvaloniaProperty.Register<MultiSliderValuesBar, Orientation>(nameof(Orientation));

	public static readonly StyledProperty<decimal> RangeProperty =
		MultiTrack.RangeProperty.AddOwner<MultiSliderValuesBar>();

	static MultiSliderValuesBar()
	{
		AffectsArrange<MultiSliderValuesBar>(ValuesProperty);
	}

	public string? StringFormat
	{
		get => GetValue(StringFormatProperty);
		set => SetValue(StringFormatProperty, value);
	}

	public ImmutableList<decimal> Values
	{
		get => GetValue(ValuesProperty);
		set => SetValue(ValuesProperty, value);
	}

	public ControlTheme? TextBlockTheme
	{
		get => GetValue(TextBlockThemeProperty);
		set => SetValue(TextBlockThemeProperty, value);
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

	public MultiSliderValuesBar()
	{
		AddTextBlocks((byte)Values.Count);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ValuesProperty)
			OnValuesChanged(change.GetOldValue<ImmutableList<decimal>>());
		else if (change.Property == StringFormatProperty)
			UpdateText();
		else if (change.Property == TextBlockThemeProperty)
			foreach (var textBlock in LogicalChildren.Cast<TextBlock>())
				textBlock.Theme = TextBlockTheme;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		Size desiredSize = new();
		foreach (var textBlock in LogicalChildren.Cast<TextBlock>())
		{
			textBlock.Measure(availableSize);
			var textBlockDesiredSize = textBlock.DesiredSize;
			desiredSize = StackSizes(desiredSize, textBlockDesiredSize);
			availableSize = SubtractStackedSize(availableSize, textBlockDesiredSize);
		}
		return desiredSize;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		double passedLength = 0;
		var availableLength = Orientation == Orientation.Horizontal ? finalSize.Width : finalSize.Height;
		foreach (var (textBlock, length) in LogicalChildren.Cast<TextBlock>().Zip(GetArrangeLengths(availableLength)))
		{
			textBlock.Arrange(GetArrangeRectangle(finalSize, length, passedLength));
			passedLength += length;
		}
		return finalSize;
	}

	private void OnValuesChanged(ImmutableList<decimal> oldValues)
	{
		var countDelta = Values.Count - oldValues.Count;
		if (countDelta > 0)
			AddTextBlocks((byte)countDelta);
		else if (countDelta < 0)
			RemoveLastChildren((byte)-countDelta);
		UpdateText();
	}

	private void AddTextBlocks(byte count)
	{
		for (byte i = 0; i < count; i++)
			AddTextBlock();
	}

	private void AddTextBlock()
	{
		AddChild(CreateTextBlock());
	}

	private TextBlock CreateTextBlock() => new()
	{
		Theme = TextBlockTheme
	};

	private void AddChild(Visual item)
	{
		LogicalChildren.Add(item);
		VisualChildren.Add(item);
	}

	private void RemoveLastChildren(byte count)
	{
		var firstChildIndex = Values.Count - count;
		LogicalChildren.RemoveRange(firstChildIndex, count);
		VisualChildren.RemoveRange(firstChildIndex, count);
	}

	private void UpdateText()
	{
		foreach (var (textBlock, value) in LogicalChildren.Cast<TextBlock>().Zip(Values))
			textBlock.Text = value.ToString(StringFormat);
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

	private IEnumerable<double> GetArrangeLengths(double availableLength)
	{
		var minimumLengths = LogicalChildren
			.Cast<TextBlock>()
			.Select(textBlock => textBlock.DesiredSize)
			.Select<Size, double>(Orientation == Orientation.Horizontal ? size => size.Width : size => size.Height)
			.ToList();
		var desiredLengths = Values
			.Select(value => (double)value / (double)Range * availableLength)
			.ToList();

		if (desiredLengths.Zip(minimumLengths).All(tuple => tuple.First > tuple.Second))
			return desiredLengths;

		var minimumLengthsSum = minimumLengths.Sum();
		if (minimumLengthsSum > availableLength)
			return minimumLengths;

		var result = desiredLengths.ToList();
		for (var i = 0; i < result.Count; i++)
		{
			var minimumLength = minimumLengths[i];
			var desiredLength = result[i];
			if (desiredLength >= minimumLength)
				continue;
			var requiredExtension = minimumLength - desiredLength;

			var previousShrinkage = ShrinkNeighbors(i, requiredExtension / 2, false);
			var nextShrinkage = ShrinkNeighbors(i, requiredExtension - previousShrinkage, true);

			var totalExtension = previousShrinkage + nextShrinkage;

			if (totalExtension < requiredExtension)
			{
				var additionalPreviousShrinkage = ShrinkNeighbors(i, requiredExtension - totalExtension, false);
				previousShrinkage += additionalPreviousShrinkage;
				totalExtension = previousShrinkage + nextShrinkage;
			}

			result[i] += totalExtension;
		}

		return result;

		double ShrinkNeighbors(int index, double desiredShrinkage, bool forward)
		{
			double actualShrinkage = 0;
			if (forward)
				for (var i = index + 1; i < result.Count; i++)
				{
					var remainingShrinkage = desiredShrinkage - actualShrinkage;
					actualShrinkage += ShrinkAt(i, remainingShrinkage);
				}
			else
				for (var i = index - 1; i >= 0; i--)
				{
					var remainingShrinkage = desiredShrinkage - actualShrinkage;
					actualShrinkage += ShrinkAt(i, remainingShrinkage);
				}
			return actualShrinkage;
		}

		double ShrinkAt(int index, double desiredShrinkage)
		{
			var extraLength = GetExtraLength(index);
			if (extraLength == 0)
				return 0;
			var shrinkage = Math.Min(extraLength, desiredShrinkage);
			result[index] -= shrinkage;
			return shrinkage;
		}

		double GetExtraLength(int index)
		{
			if (index < 0 || index >= result.Count)
				return 0;
			return Math.Max(0, result[index] - minimumLengths[index]);
		}
	}

	private Rect GetArrangeRectangle(Size finalSize, double length, double passedLength)
	{
		return Orientation == Orientation.Horizontal
			? new Rect(passedLength, 0, length, finalSize.Height)
			: new Rect(0, passedLength, finalSize.Width, length);
	}
}