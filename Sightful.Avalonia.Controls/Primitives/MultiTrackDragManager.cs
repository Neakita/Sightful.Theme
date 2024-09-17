using System.Collections.Immutable;
using Avalonia.Input;
using Avalonia.Layout;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia.Controls.Primitives;

internal sealed class MultiTrackDragManager
{
	public MultiTrackDragManager(MultiTrack track, MultiTrackArranger arranger)
	{
		_track = track;
		_arranger = arranger;
	}

	public void OnPointerPressed(PointerPressedEventArgs args)
	{
		if (_session != null)
			return;
		var position = args.GetPosition(_track);
		var normalizedLength = _track.Orientation == Orientation.Horizontal
			? position.X / _track.Bounds.Width
			: position.Y / _track.Bounds.Height;
		var length = (decimal)normalizedLength * _track.Range;
		var valueIndex = GetDragIndex(length);
		decimal accumulated = 0;

		for (var i = 0; i < valueIndex; i++)
		{
			var value = _track.Values[i];
			accumulated += value;
		}

		var newValue = length - accumulated;
		_track.Values = Shift(_track.Values, valueIndex, newValue - _track.Values[valueIndex]);
		_session = new MultiTrackDragSession(valueIndex, position);
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

	private byte GetDragIndex(decimal length)
	{
		var values = _track.Values;
		List<decimal> thumbPositions = new(values.Count - 1);
		decimal accumulation = 0;
		for (var i = 0; i < values.Count - 1; i++)
		{
			var value = values[i];
			accumulation += value;
			thumbPositions.Add(accumulation);
		}
		if (thumbPositions.Count == 1)
			return 0;
		for (byte i = 0; i < thumbPositions.Count - 1; i++)
		{
			var currentThumbLength = thumbPositions[i];
			var nextThumbLength = thumbPositions[i + 1];
			var currentThumbDistance = Math.Abs(currentThumbLength - length);
			var nextThumbDistance = Math.Abs(nextThumbLength - length);
			if (currentThumbDistance <= nextThumbDistance)
				return i;
		}
		return (byte)(thumbPositions.Count - 1);
	}
}