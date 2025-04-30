using Avalonia;
using Avalonia.Controls.Presenters;
using Avalonia.Media.Transformation;

namespace Sightful.Avalonia.Controls.ZoomViewer;

public sealed class ZoomContentPresenter : ContentPresenter
{
	public static readonly StyledProperty<double> ZoomProperty =
		ZoomViewer.ZoomProperty.AddOwner<ZoomContentPresenter>();

	public static readonly StyledProperty<Vector> OffsetProperty =
		ZoomViewer.OffsetProperty.AddOwner<ZoomContentPresenter>();

	public double Zoom
	{
		get => GetValue(ZoomProperty);
		set => SetValue(ZoomProperty, value);
	}

	public Vector Offset
	{
		get => GetValue(OffsetProperty);
		set => SetValue(OffsetProperty, value);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		var property = change.Property;
		if (property == ZoomProperty || property == OffsetProperty || property == ChildProperty)
			UpdateChildTransform();
	}

	private void UpdateChildTransform()
	{
		if (Child == null)
			return;
		var builder = TransformOperations.CreateBuilder(2);
		builder.AppendScale(Zoom, Zoom);
		builder.AppendTranslate(-Offset.X * Zoom, -Offset.Y * Zoom);
		var transform = builder.Build();
		Child.RenderTransform = transform;
	}
}