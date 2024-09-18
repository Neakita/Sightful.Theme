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
		AffectsMeasure<MultiSliderValuesBar>(ValuesProperty);
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
		_childrenManager = new MultiSliderValuesBarChildrenManager(LogicalChildren, VisualChildren, Values);
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
		{
			_childrenManager.Values = Values;
			_arranger.Values = Values;
		}
		else if (change.Property == StringFormatProperty)
			_childrenManager.StringFormat = StringFormat;
		else if (change.Property == TextBlockThemeProperty)
			_childrenManager.TextBlockTheme = TextBlockTheme;
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

	private readonly MultiSliderValuesBarChildrenManager _childrenManager;
	private readonly MultiSliderValuesBarArranger _arranger;
}