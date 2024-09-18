namespace Sightful.Avalonia.Controls.Primitives;

internal sealed class MultiTrackDragSession
{
	public byte ValueIndex { get; }
	public decimal ValuePosition { get; }

	public MultiTrackDragSession(byte valueIndex, decimal valuePosition)
	{
		ValueIndex = valueIndex;
		ValuePosition = valuePosition;
	}
}