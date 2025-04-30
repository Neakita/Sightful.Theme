using Avalonia;
using Avalonia.Controls.Presenters;

namespace Sightful.Avalonia.Controls.ZoomViewer;

internal sealed class ForegroundContentPresenter : ContentPresenter
{
	public static readonly StyledProperty<Size> ContentSizeProperty =
		AvaloniaProperty.Register<ForegroundContentPresenter, Size>(nameof(ContentSize));

	public static readonly StyledProperty<double> ScaleProperty =
		ZoomViewer.ZoomProperty.AddOwner<ForegroundContentPresenter>();

	public static readonly StyledProperty<Vector> OffsetProperty =
		ZoomViewer.OffsetProperty.AddOwner<ForegroundContentPresenter>();

	static ForegroundContentPresenter()
	{
		AffectsArrange<ForegroundContentPresenter>(ScaleProperty, OffsetProperty);
	}

	public Size ContentSize
	{
		get => GetValue(ContentSizeProperty);
		set => SetValue(ContentSizeProperty, value);
	}

	public double Scale
	{
		get => GetValue(ScaleProperty);
		set => SetValue(ScaleProperty, value);
	}

	public Vector Offset
	{
		get => GetValue(OffsetProperty);
		set => SetValue(OffsetProperty, value);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		if (Child == null)
			return finalSize;
		var center = new Point(finalSize.Width / 2, finalSize.Height / 2);
		var scaledSize = ContentSize * Scale;
		var topLeft = new Point(
			center.X - scaledSize.Width / 2 - Offset.X * Scale,
			center.Y - scaledSize.Height / 2 - Offset.Y * Scale);
		var bounds = new Rect(topLeft, scaledSize);
		Child.Arrange(bounds);
		return finalSize;
	}
}