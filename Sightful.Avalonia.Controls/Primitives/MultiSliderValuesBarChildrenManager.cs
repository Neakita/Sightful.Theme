using System.Collections.Immutable;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Styling;

namespace Sightful.Avalonia.Controls.Primitives;

internal sealed class MultiSliderValuesBarChildrenManager
{
	public IReadOnlyCollection<decimal> Values
	{
		get => _values;
		set
		{
			var countDelta = value.Count - _values.Count;
			_values = value;
			if (countDelta > 0)
				AddTextBlocks((byte)countDelta);
			else if (countDelta < 0)
				RemoveLastChildren((byte)-countDelta);
			UpdateText();
		}
	}

	public ControlTheme? TextBlockTheme
	{
		get => _textBlockTheme;
		set
		{
			_textBlockTheme = value;
			foreach (var textBlock in _logicalChildren.Cast<TextBlock>())
				textBlock.Theme = value;
		}
	}

	public string? StringFormat
	{
		get => _stringFormat;
		set
		{
			_stringFormat = value;
			UpdateText();
		}
	}

	public MultiSliderValuesBarChildrenManager(
		IAvaloniaList<ILogical> logicalChildren,
		IAvaloniaList<Visual> visualChildren,
		IReadOnlyCollection<decimal> values)
	{
		_logicalChildren = logicalChildren;
		_visualChildren = visualChildren;
		Values = values;
	}

	private readonly IAvaloniaList<ILogical> _logicalChildren;
	private readonly IAvaloniaList<Visual> _visualChildren;
	private IReadOnlyCollection<decimal> _values = ImmutableList<decimal>.Empty;
	private ControlTheme? _textBlockTheme;
	private string? _stringFormat;

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
		_logicalChildren.Add(item);
		_visualChildren.Add(item);
	}

	private void RemoveLastChildren(byte count)
	{
		var firstChildIndex = Values.Count - count;
		_logicalChildren.RemoveRange(firstChildIndex, count);
		_visualChildren.RemoveRange(firstChildIndex, count);
	}

	private void UpdateText()
	{
		foreach (var (textBlock, value) in _logicalChildren.Cast<TextBlock>().Zip(Values))
			textBlock.Text = value.ToString(StringFormat);
	}
}