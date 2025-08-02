using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia.Controls.Primitives;

internal sealed class MultiTrackArranger : StackArranger
{
	public IReadOnlyList<decimal> Values { get; set; }
	public decimal Range { get; set; }
	public decimal Density { get; private set; }

	public MultiTrackArranger(IReadOnlyList<decimal> values, IReadOnlyList<ILogical> logicalChildren, decimal range)
	{
		Values = values;
		_logicalChildren = logicalChildren;
		Range = range;
	}

	public Size Measure(Size availableSize)
	{
		Size desiredSize = new();
		foreach (var thumb in _logicalChildren.OfType<Thumb>())
		{
			thumb.Measure(availableSize);
			desiredSize = StackSizes(desiredSize, thumb.DesiredSize);
			availableSize = SubtractStackedSize(availableSize, thumb.DesiredSize);
		}

		return desiredSize;
	}

	public Size Arrange(Size finalSize)
	{
		var availableLength = Orientation == Orientation.Horizontal ? finalSize.Width : finalSize.Height;
		var thumbsLength = _logicalChildren.OfType<Thumb>().Select(GetDesiredLength).Sum();
		Density = (decimal)(availableLength - thumbsLength);
		double passedLength = 0;
		foreach (var (child, length) in _logicalChildren.Cast<Layoutable>().Zip(GetPiecesLengths()))
		{
			child.Arrange(GetArrangeRectangle(finalSize, passedLength, (double)length));
			passedLength += (double)length;
		}

		return Orientation == Orientation.Horizontal
			? finalSize.WithWidth(passedLength)
			: finalSize.WithHeight(passedLength);
	}

	private readonly IReadOnlyList<ILogical> _logicalChildren;

	private double GetDesiredLength(Layoutable layoutable)
	{
		return Orientation == Orientation.Horizontal ? layoutable.DesiredSize.Width : layoutable.DesiredSize.Height;
	}

	private IEnumerable<decimal> GetPiecesLengths()
	{
		for (var i = 0; i < _logicalChildren.Count; i++)
		{
			var child = _logicalChildren[i];
			var isRangeButton = i % 2 == 0;
			if (isRangeButton)
			{
				Guard.IsOfType<Button>(child);
				var value = Values[i / 2];
				var normalizedValue = value / Range;
				yield return Density * normalizedValue;
			}
			else yield return (decimal)GetDesiredLength((Thumb)child);
		}
	}

	private Rect GetArrangeRectangle(Size finalSize, double passedLength, double length)
	{
		return Orientation == Orientation.Horizontal
			? new Rect(passedLength, 0, length, finalSize.Height)
			: new Rect(0, passedLength, finalSize.Width, length);
	}
}