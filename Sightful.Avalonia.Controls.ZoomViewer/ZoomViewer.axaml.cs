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
		AvaloniaProperty.Register<ZoomViewer, Vector>(nameof(Offset), defaultBindingMode: BindingMode.TwoWay, coerce: CoerceOffset);

	public static readonly StyledProperty<object?> ForegroundContentProperty =
		AvaloniaProperty.Register<ZoomViewer, object?>(nameof(ForegroundContent));

	public static readonly StyledProperty<IDataTemplate?> ForegroundContentTemplateProperty =
		AvaloniaProperty.Register<ZoomViewer, IDataTemplate?>(nameof(ForegroundContentTemplate));

	public static readonly DirectProperty<ZoomViewer, Vector> MinOffsetProperty =
		AvaloniaProperty.RegisterDirect<ZoomViewer, Vector>(nameof(MinOffset), viewer => viewer.MinOffset);

	public static readonly DirectProperty<ZoomViewer, Vector> MaxOffsetProperty =
		AvaloniaProperty.RegisterDirect<ZoomViewer, Vector>(nameof(MaxOffset), viewer => viewer.MaxOffset);

	private const string ContentPresenterName = "PART_ContentPresenter";
	private const string ForegroundContentPresenterName = "PART_ForegroundContentPresenter";

	private static double CoerceZoom(AvaloniaObject element, double zoom)
	{
		return Math.Max(zoom, 1);
	}

	private static Vector CoerceOffset(AvaloniaObject element, Vector offset)
	{
		var (minX, minY) = element.GetValue(MinOffsetProperty);
		var (maxX, maxY) = element.GetValue(MaxOffsetProperty);
		return new Vector(
			Math.Clamp(offset.X, minX, maxX),
			Math.Clamp(offset.Y, minY, maxY));
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

	public Vector MinOffset
	{
		get => _minOffset;
		private set => SetAndRaise(MinOffsetProperty, ref _minOffset, value);
	}

	public Vector MaxOffset
	{
		get => _maxOffset;
		private set => SetAndRaise(MaxOffsetProperty, ref _maxOffset, value);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		_contentSizeBinding?.Dispose();
		_contentPresenter = e.NameScope.Find<ContentPresenter>(ContentPresenterName);
		var foregroundContentPresenter = e.NameScope.Find<ContentPresenter>(ForegroundContentPresenterName);
		if (_contentPresenter == null || foregroundContentPresenter == null)
			return;
		var contentSizeObservable = _contentPresenter
			.GetObservable(ContentPresenter.ChildProperty)
			.Where(child => child != null)
			.Select(child => child!)
			.Select(child => child.GetObservable(BoundsProperty))
			.Switch()
			.Select(bounds => bounds.Size);
		_contentSizeBinding = foregroundContentPresenter.Bind(ForegroundContentPresenter.ContentSizeProperty, contentSizeObservable);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ZoomProperty)
		{
			if (_contentPresenter == null || _contentPresenter.Child == null)
				return;
			var viewport = _contentPresenter.Bounds.Size;
			var contentSize = _contentPresenter.Child.Bounds.Size * Zoom;
			var excess = contentSize - viewport;
			excess = new Size(
				Math.Max(0, excess.Width),
				Math.Max(0, excess.Height));
			MaxOffset = new Vector(excess.Width / 2 / Zoom, excess.Height / 2 / Zoom);
			MinOffset = -MaxOffset;
		}
	}

	protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
	{
		SetCurrentValue(ZoomProperty, Zoom + e.Delta.Y);
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		_lastPanPosition = e.GetPosition(this);
		PointerMoved += OnPointerMoved;
		PointerReleased += OnPointerReleased;
	}

	private ContentPresenter? _contentPresenter;
	private IDisposable? _contentSizeBinding;
	private Point _lastPanPosition;
	private Vector _minOffset;
	private Vector _maxOffset;

	private void OnPointerMoved(object? sender, PointerEventArgs e)
	{
		var panPosition = e.GetPosition(this);
		var panDelta = (Vector)(_lastPanPosition - panPosition) / Zoom;
		_lastPanPosition = panPosition;
		SetCurrentValue(OffsetProperty, Offset + panDelta);
	}

	private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		PointerMoved -= OnPointerMoved;
		PointerReleased -= OnPointerReleased;
	}
}