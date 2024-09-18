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
		_arranger = new MultiSliderValuesBarArranger(LogicalChildren)
		{
			Orientation = Orientation,
			Values = Values,
			Range = Range
		};
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
		else if (change.Property == OrientationProperty)
			_arranger.Orientation = Orientation;
		else if (change.Property == RangeProperty)
			_arranger.Range = Range;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		return _arranger.Measure(availableSize);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		return _arranger.Arrange(finalSize);
	}

	private readonly MultiSliderValuesBarArranger _arranger;

	private void OnValuesChanged(ImmutableList<decimal> oldValues)
	{
		var countDelta = Values.Count - oldValues.Count;
		if (countDelta > 0)
			AddTextBlocks((byte)countDelta);
		else if (countDelta < 0)
			RemoveLastChildren((byte)-countDelta);
		UpdateText();
		_arranger.Values = Values;
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
}