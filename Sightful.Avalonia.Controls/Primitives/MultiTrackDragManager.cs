using System.Collections.Immutable;
using Avalonia.Input;
using Avalonia.Layout;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia.Controls.Primitives;

internal sealed class MultiTrackDragManager
{
	public MultiTrackDragManager(MultiTrack track)
	{
		_track = track;
	}

	public void OnPointerPressed(PointerPressedEventArgs args)
	{
		if (_session != null)
			return;
		var pointerPosition = args.GetPosition(_track);
		var normalizedPointerPosition = _track.Orientation == Orientation.Horizontal
			? pointerPosition.X / _track.Bounds.Width
			: pointerPosition.Y / _track.Bounds.Height;
		var rangedPointerPosition = (decimal)normalizedPointerPosition * _track.Range;
		var valueIndex = GetDragIndex(rangedPointerPosition);
		var valuePosition = _track.Values.Take(valueIndex).Sum();
		var newValue = rangedPointerPosition - valuePosition;
		_track.Values = SetValue(_track.Values, valueIndex, newValue);
		_session = new MultiTrackDragSession(valueIndex, valuePosition);
	}

	public void OnPointerMoved(PointerEventArgs args)
	{
		if (_session == null)
			return;
		var pointerPosition = args.GetPosition(_track);
		var normalizedPointerPosition = _track.Orientation == Orientation.Horizontal
			? pointerPosition.X / _track.Bounds.Width
			: pointerPosition.Y / _track.Bounds.Height;
		var rangedPointerPosition = (decimal)normalizedPointerPosition * _track.Range;
		var valuePosition = _track.Values.Take(_session.ValueIndex).Sum();
		var newValue = rangedPointerPosition - valuePosition;
		_track.Values = SetValue(_track.Values, _session.ValueIndex, newValue);
	}

	public void OnPointerReleased()
	{
		_session = null;
	}

	private readonly MultiTrack _track;
	private MultiTrackDragSession? _session;

	private ImmutableList<decimal> SetValue(ImmutableList<decimal> values, int index, decimal newValue)
	{
		newValue = SnapValue(newValue);
		var oldValue = values[index];
		var delta = newValue - oldValue;
		return Shift(values, index, delta);
	}

	private ImmutableList<decimal> Shift(ImmutableList<decimal> values, int index, decimal delta)
	{
		delta = Math.Clamp(delta, -values[index] + _track.MinimumValue, values[index + 1] - _track.MinimumValue);
		Guard.IsEqualTo(values.Sum(), _track.Range);
		var builder = values.ToBuilder();
		builder[index] += delta;
		builder[index + 1] -= delta;
		Guard.IsEqualTo(builder.Sum(), _track.Range);
		foreach (var value in builder)
			Guard.IsGreaterThanOrEqualTo(value, 0);
		return builder.ToImmutable();
	}

	private byte GetDragIndex(decimal rangedPosition)
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
			var currentThumbDistance = Math.Abs(currentThumbLength - rangedPosition);
			var nextThumbDistance = Math.Abs(nextThumbLength - rangedPosition);
			if (currentThumbDistance <= nextThumbDistance)
				return i;
		}
		return (byte)(thumbPositions.Count - 1);
	}

	private decimal SnapValue(decimal value)
	{
		if (_track.Increment <= 0)
			return value;
		var previous = Math.Round(value / _track.Increment) * _track.Increment;
		var next = Math.Min(_track.Range, previous + _track.Increment);
		var previousDistance = value - previous;
		var nextDistance = next - value;
		return nextDistance > previousDistance ? previous : next;
	}
}