using System.Collections.Immutable;
using Avalonia;
using Avalonia.Controls;

namespace Sightful.Avalonia.Controls;

// Uneven? Irregular? Bumpy?
public sealed class JaggedWrapPanel : Panel
{
	protected override Size MeasureOverride(Size availableSize)
	{
		foreach (var child in Children)
			child.Measure(availableSize);
		var lines = Distribute(availableSize);
		return new Size(lines.Max(line => line.Width), lines.Sum(line => line.Groups.Max(group => group.Height)));
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		var lines = Distribute(finalSize);
		foreach (var (line, lineIndex) in lines.Select((line, index) => (line, index)))
		{
			foreach (var (group, groupIndex) in line.Groups.Select((group, index) => (group, index)))
			{
				foreach (var (child, indexInGroup) in group.Controls.Select((control, index) => (control, index)))
				{
					var x = lines[lineIndex].Groups.Take(groupIndex).Sum(g => g.Controls.Max(control => control.DesiredSize.Width));
					var y = lines.Take(lineIndex).Sum(l => l.Groups.Max(g => g.Height)) + lines[lineIndex]
						.Groups[groupIndex].Controls.Take(indexInGroup).Sum(control => control.DesiredSize.Height);
					Point point = new(x, y);
					Rect rect = new(point, child.DesiredSize);
					child.Arrange(rect);
				}
			}
		}
		return finalSize;
	}

	private ImmutableList<Line> Distribute(Size availableSize)
	{
		if (Children.Count == 0)
			return ImmutableList<Line>.Empty;
		var builder = ImmutableList.CreateBuilder<Line>();
		builder.Add(new Line(availableSize.Width));
		foreach (var child in Children)
		{
			if (builder.All(line => !line.Add(child)))
			{
				Line newLine = new(availableSize.Width);
				newLine.Add(child);
				builder.Add(newLine);
			}
		}
		return builder.ToImmutable();
	}

	private sealed class Line
	{
		public List<Group> Groups { get; } = new();

		public double Width { get; private set; }

		public Line(double availableWidth)
		{
			_availableWidth = availableWidth;
		}

		public bool Add(Control control)
		{
			var desiredHeight = control.DesiredSize.Height;
			_highestControlHeight = Math.Max(_highestControlHeight, desiredHeight);
			var fullestSuitableGroup = Groups
				.Where(group => group.Height + desiredHeight <= _highestControlHeight)
				.MaxBy(group => group.Height);
			if (fullestSuitableGroup != null)
			{
				fullestSuitableGroup.Add(control);
				return true;
			}
			var currentAvailableWidth = _availableWidth - Width;
			if (currentAvailableWidth < control.DesiredSize.Width)
				return false;
			Group newGroup = new();
			newGroup.Add(control);
			Groups.Add(newGroup);
			Width += control.DesiredSize.Width;
			return true;
		}

		private readonly double _availableWidth;
		private double _highestControlHeight;
	}

	private sealed class Group
	{
		public List<Control> Controls { get; } = new();

		public double Height { get; private set; }

		public void Add(Control control)
		{
			Controls.Add(control);
			Height += control.DesiredSize.Height;
		}
	}
}