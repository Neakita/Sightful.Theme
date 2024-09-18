using System.Collections.Immutable;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;

namespace Sightful.Avalonia.Controls.Primitives;

internal sealed class MultiSliderValuesBarArranger : StackArranger
{
	public IReadOnlyCollection<decimal> Values { get; set; } = ImmutableList<decimal>.Empty;
	public decimal Range { get; set; }

	public MultiSliderValuesBarArranger(IReadOnlyCollection<ILogical> logicalChildren)
	{
		_logicalChildren = logicalChildren;
	}
	
	public Size Measure(Size availableSize)
	{
		Size desiredSize = new();
		foreach (var textBlock in _logicalChildren.Cast<TextBlock>())
		{
			textBlock.Measure(availableSize);
			var textBlockDesiredSize = textBlock.DesiredSize;
			desiredSize = StackSizes(desiredSize, textBlockDesiredSize);
			availableSize = SubtractStackedSize(availableSize, textBlockDesiredSize);
		}
		return desiredSize;
	}
	
	public Size Arrange(Size finalSize)
	{
		double passedLength = 0;
		var availableLength = Orientation == Orientation.Horizontal ? finalSize.Width : finalSize.Height;
		foreach (var (textBlock, length) in _logicalChildren.Cast<TextBlock>().Zip(GetArrangeLengths(availableLength)))
		{
			textBlock.Arrange(GetArrangeRectangle(finalSize, length, passedLength));
			passedLength += length;
		}
		return finalSize;
	}

	private readonly IReadOnlyCollection<ILogical> _logicalChildren;

	private IEnumerable<double> GetArrangeLengths(double availableLength)
	{
		var minimumLengths = _logicalChildren
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