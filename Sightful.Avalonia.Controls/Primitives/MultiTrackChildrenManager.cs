using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Styling;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia.Controls.Primitives;

internal sealed class MultiTrackChildrenManager
{
	public byte ValuesCount
	{
		get => _valuesCount;
		set
		{
			Guard.IsGreaterThanOrEqualTo<byte>(value, MinimumValuesCount);
			var delta = value - _valuesCount;
			if (delta > 0)
				AddValuesPieces((byte)delta);
			else if (delta < 0)
				RemoveValuePieces((byte)-delta);
			_valuesCount = value;
		}
	}

	public ControlTheme? RangeButtonTheme
	{
		get => _rangeButtonTheme;
		set
		{
			_rangeButtonTheme = value;
			foreach (var button in _logicalChildren.OfType<Button>())
				button.Theme = value;
		}
	}

	public ControlTheme? ThumbTheme
	{
		get => _thumbTheme;
		set
		{
			_thumbTheme = value;
			foreach (var thumb in _logicalChildren.OfType<Thumb>())
				thumb.Theme = value;
		}
	}

	public MultiTrackChildrenManager(IAvaloniaList<ILogical> logicalChildren, IAvaloniaList<Visual> visualChildren)
	{
		_logicalChildren = logicalChildren;
		_visualChildren = visualChildren;
		AddChild(CreateRangeButton());
		byte remainingValuesCount = (byte)(ValuesCount - 1);
		AddValuesPieces(remainingValuesCount);
	}

	private const byte MinimumValuesCount = MultiTrack.MinimumValuesCount;
	private readonly IAvaloniaList<ILogical> _logicalChildren;
	private readonly IAvaloniaList<Visual> _visualChildren;
	private byte _valuesCount = MinimumValuesCount;
	private ControlTheme? _rangeButtonTheme;
	private ControlTheme? _thumbTheme;

	private void AddValuesPieces(byte newValuesCount)
	{
		for (byte i = 0; i < newValuesCount; i++)
		{
			AddChild(CreateRangeButton());
			AddChild(CreateThumb());
		}
	}

	private Button CreateRangeButton() => new()
	{
		Theme = RangeButtonTheme
	};

	private Thumb CreateThumb() => new()
	{
		Theme = ThumbTheme
	};

	private void AddChild(Visual item)
	{
		_logicalChildren.Add(item);
		_visualChildren.Add(item);
	}

	private void RemoveValuePieces(byte removedValuesCount)
	{
		var piecesCount = removedValuesCount * 2; // range button and thumb for each
		var firstPieceIndex = _logicalChildren.Count - piecesCount;
		_logicalChildren.RemoveRange(firstPieceIndex, piecesCount);
		_visualChildren.RemoveRange(firstPieceIndex, piecesCount);
	}
}