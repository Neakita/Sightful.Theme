using Avalonia;

namespace Sightful.Avalonia.Controls.Primitives;

internal sealed class MultiTrackDragSession
{
	public byte ValueIndex { get; }
	public Point PreviousPointerPosition { get; set; }

	public MultiTrackDragSession(byte valueIndex, Point previousPointerPosition)
	{
		ValueIndex = valueIndex;
		PreviousPointerPosition = previousPointerPosition;
	}
}