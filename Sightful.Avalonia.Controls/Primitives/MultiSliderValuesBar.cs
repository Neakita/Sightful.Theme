using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Layout;
using Avalonia.Styling;

namespace Sightful.Avalonia.Controls.Primitives;

[PseudoClasses(":vertical", ":horizontal")]
public sealed class MultiSliderValuesBar : Control
{
	public static readonly StyledProperty<ControlTheme?> TextBlockThemeProperty =
		AvaloniaProperty.Register<MultiSliderValuesBar, ControlTheme?>(nameof(TextBlockTheme));

	public static readonly StyledProperty<string?> StringFormatProperty =
		AvaloniaProperty.Register<MultiSliderValuesBar, string?>(nameof(StringFormat));

	public static readonly StyledProperty<Orientation> OrientationProperty =
		AvaloniaProperty.Register<MultiSliderValuesBar, Orientation>(nameof(Orientation));

	public static readonly StyledProperty<decimal> RangeProperty =
		MultiTrack.RangeProperty.AddOwner<MultiSliderValuesBar>();

	public static readonly StyledProperty<IReadOnlyCollection<decimal>> SizeFractionsProperty =
		AvaloniaProperty.Register<MultiSliderValuesBar, IReadOnlyCollection<decimal>>(nameof(SizeFractions));

	public static readonly StyledProperty<IReadOnlyCollection<object?>> ValuesProperty =
		AvaloniaProperty.Register<MultiSliderValuesBar, IReadOnlyCollection<object?>>(nameof(Values), defaultValue: ReadOnlyCollection<object?>.Empty);

	static MultiSliderValuesBar()
	{
		AffectsMeasure<MultiSliderValuesBar>(ValuesProperty);
		AffectsArrange<MultiSliderValuesBar>(ValuesProperty);
		AffectsMeasure<MultiSliderValuesBar>(SizeFractionsProperty);
		AffectsArrange<MultiSliderValuesBar>(SizeFractionsProperty);
	}

	public string? StringFormat
	{
		get => GetValue(StringFormatProperty);
		set => SetValue(StringFormatProperty, value);
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

	public IReadOnlyCollection<decimal> SizeFractions
	{
		get => GetValue(SizeFractionsProperty);
		set => SetValue(SizeFractionsProperty, value);
	}

	public IReadOnlyCollection<object?> Values
	{
		get => GetValue(ValuesProperty);
		set => SetValue(ValuesProperty, value);
	}

	public MultiSliderValuesBar()
	{
		_childrenManager = new MultiSliderValuesBarChildrenManager(LogicalChildren, VisualChildren, Values);
		_arranger = new MultiSliderValuesBarArranger(LogicalChildren)
		{
			Orientation = Orientation,
			SizeFractions = SizeFractions,
			Range = Range
		};
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ValuesProperty)
			_childrenManager.Values = Values;
		else if (change.Property == SizeFractionsProperty)
			_arranger.SizeFractions = SizeFractions;
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