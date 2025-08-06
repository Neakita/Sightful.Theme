namespace Sightful.Avalonia.Controls.GestureBox;

public sealed class GestureEdit
{
	public object? Gesture { get; }
	public bool ShouldStopEditing { get; }

	public GestureEdit(object? gesture, bool shouldStopEditing)
	{
		Gesture = gesture;
		ShouldStopEditing = shouldStopEditing;
	}
}