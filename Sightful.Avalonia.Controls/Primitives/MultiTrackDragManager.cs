using System.Collections.Immutable;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia.Controls.Primitives;

internal sealed class MultiTrackDragManager
{
	public MultiTrackDragManager(MultiTrack track, IReadOnlyCollection<ILogical> logicalChildren, MultiTrackArranger arranger)
	{
		_track = track;
		_logicalChildren = logicalChildren;
		_arranger = arranger;
	}

	public void OnPointerPressed(PointerPressedEventArgs args)
	{
		if (_session != null)
			return;
		var position = args.GetPosition(_track);
		Point normalizedPosition = new(position.X / _track.Bounds.Width, position.Y / _track.Bounds.Height);
		var normalizedLength = _track.Orientation == Orientation.Horizontal ? normalizedPosition.X : normalizedPosition.Y;
		var length = (decimal)normalizedLength * _track.Range;
		var valueIndex = (byte?)_logicalChildren
			.OfType<Thumb>()
			.Select((thumb, index) => (thumb, index))
			.MinBy(tuple =>
			{
				var thumbPosition = args.GetPosition(tuple.thumb);
				var distance = _track.Orientation == Orientation.Horizontal ? thumbPosition.X : thumbPosition.Y;
				return Math.Abs(distance);
			}).index;
		Guard.IsNotNull(valueIndex);
		
		decimal accumulated = 0;
		for (var i = 0; i < valueIndex; i++)
		{
			var value = _track.Values[i];
			accumulated += value;
		}

		var newValue = length - accumulated;
		_track.Values = Shift(_track.Values, valueIndex.Value, newValue - _track.Values[valueIndex.Value]);
		_session = new MultiTrackDragSession(valueIndex.Value, position);
	}

	public void OnPointerMoved(PointerEventArgs args)
	{
		if (_session == null)
			return;
		var position = args.GetPosition(_track);
		var delta = position - _session.PreviousPointerPosition;
		var distance = _track.Orientation == Orientation.Horizontal ? delta.X : delta.Y;
		Drag(_session.ValueIndex, (decimal)distance);
		_session.PreviousPointerPosition = position;
	}

	public void OnPointerReleased()
	{
		_session = null;
	}

	private readonly MultiTrack _track;
	private readonly IReadOnlyCollection<ILogical> _logicalChildren;
	private readonly MultiTrackArranger _arranger;
	private MultiTrackDragSession? _session;

	private void Drag(int index, decimal distance)
	{
		var valueDelta = distance / _arranger.Density;
		_track.Values = Shift(_track.Values, index, valueDelta);
	}

	private ImmutableList<decimal> Shift(ImmutableList<decimal> values, int index, decimal delta)
	{
		delta = Math.Clamp(delta, -values[index], values[index + 1]);
		Guard.IsEqualTo(values.Sum(), _track.Range);
		var builder = values.ToBuilder();
		builder[index] += delta;
		builder[index + 1] -= delta;
		Guard.IsEqualTo(builder.Sum(), _track.Range);
		foreach (var value in builder)
			Guard.IsGreaterThanOrEqualTo(value, 0);
		return builder.ToImmutable();
	}
}