using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;

namespace Sightful.Avalonia.Controls.ZoomViewer;

public sealed class ZoomViewer : ContentControl
{
	public static readonly StyledProperty<double> ZoomProperty =
		AvaloniaProperty.Register<ZoomViewer, double>(nameof(Zoom), 1, defaultBindingMode: BindingMode.TwoWay, coerce: CoerceZoom);

	public static readonly StyledProperty<Vector> OffsetProperty =
		AvaloniaProperty.Register<ZoomViewer, Vector>(nameof(Offset), defaultBindingMode: BindingMode.TwoWay);

	private static double CoerceZoom(AvaloniaObject element, double zoom)
	{
		return Math.Max(zoom, 1);
	}

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

	protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
	{
		SetCurrentValue(ZoomProperty, Zoom += e.Delta.Y);
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		_lastPanPosition = e.GetPosition(this);
		PointerMoved += OnPointerMoved;
		PointerReleased += OnPointerReleased;
	}

	private Point _lastPanPosition;

	private void OnPointerMoved(object? sender, PointerEventArgs e)
	{
		var panPosition = e.GetPosition(this);
		var panDelta = (Vector)(panPosition - _lastPanPosition);
		_lastPanPosition = panPosition;
		SetCurrentValue(OffsetProperty, Offset + panDelta);
	}

	private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		PointerMoved -= OnPointerMoved;
		PointerReleased -= OnPointerReleased;
	}
}