using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;

namespace Sightful.Avalonia.Controls.ZoomViewer;

[TemplatePart(ContentPresenterName, typeof(ContentPresenter))]
[TemplatePart(ForegroundContentPresenterName, typeof(ContentPresenter))]
public sealed class ZoomViewer : ContentControl
{
	public static readonly StyledProperty<double> ZoomProperty =
		AvaloniaProperty.Register<ZoomViewer, double>(nameof(Zoom), 1, defaultBindingMode: BindingMode.TwoWay, coerce: CoerceZoom);

	public static readonly StyledProperty<Vector> OffsetProperty =
		AvaloniaProperty.Register<ZoomViewer, Vector>(nameof(Offset), defaultBindingMode: BindingMode.TwoWay);

	public static readonly StyledProperty<object?> ForegroundContentProperty =
		AvaloniaProperty.Register<ZoomViewer, object?>(nameof(ForegroundContent));

	public static readonly StyledProperty<IDataTemplate?> ForegroundContentTemplateProperty =
		AvaloniaProperty.Register<ZoomViewer, IDataTemplate?>(nameof(ForegroundContentTemplate));

	private const string ContentPresenterName = "PART_ContentPresenter";
	private const string ForegroundContentPresenterName = "PART_ForegroundContentPresenter";

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

	public object? ForegroundContent
	{
		get => GetValue(ForegroundContentProperty);
		set => SetValue(ForegroundContentProperty, value);
	}

	public IDataTemplate? ForegroundContentTemplate
	{
		get => GetValue(ForegroundContentTemplateProperty);
		set => SetValue(ForegroundContentTemplateProperty, value);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		_contentSizeBinding?.Dispose();
		var contentPresenter = e.NameScope.Find<ContentPresenter>(ContentPresenterName);
		var foregroundContentPresenter = e.NameScope.Find<ContentPresenter>(ForegroundContentPresenterName);
		if (contentPresenter == null || foregroundContentPresenter == null)
			return;
		var contentSizeObservable = contentPresenter
			.GetObservable(ContentPresenter.ChildProperty)
			.Where(child => child != null)
			.Select(child => child!)
			.Select(child => child.GetObservable(BoundsProperty))
			.Switch()
			.Select(bounds => bounds.Size);
		_contentSizeBinding = foregroundContentPresenter.Bind(ForegroundContentPresenter.ContentSizeProperty, contentSizeObservable);
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

	private IDisposable? _contentSizeBinding;
	private Point _lastPanPosition;

	private void OnPointerMoved(object? sender, PointerEventArgs e)
	{
		var panPosition = e.GetPosition(this);
		var panDelta = (Vector)(panPosition - _lastPanPosition) / Zoom;
		_lastPanPosition = panPosition;
		SetCurrentValue(OffsetProperty, Offset + panDelta);
	}

	private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		PointerMoved -= OnPointerMoved;
		PointerReleased -= OnPointerReleased;
	}
}