using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Sightful.Avalonia.Controls.GestureBox;

[PseudoClasses(EmptyPseudoClass, EditingPseudoClass)]
public sealed class GestureBox : TemplatedControl
{
	public static readonly StyledProperty<object?> GestureProperty =
		AvaloniaProperty.Register<GestureBox, object?>(nameof(Gesture), defaultBindingMode: BindingMode.TwoWay);

	public static readonly StyledProperty<IObservable<GestureEdit>> GestureEditsObservableProperty =
		AvaloniaProperty.Register<GestureBox, IObservable<GestureEdit>>(nameof(GestureEditsObservable));

	public static readonly StyledProperty<string?> WatermarkProperty =
		AvaloniaProperty.Register<GestureBox, string?>(nameof(Watermark));

	static GestureBox()
	{
		FocusableProperty.OverrideDefaultValue<GestureBox>(true);
	}

	private const string EmptyPseudoClass = ":empty";
	private const string EditingPseudoClass = ":editing";

	public object? Gesture
	{
		get => GetValue(GestureProperty);
		set => SetValue(GestureProperty, value);
	}

	public IObservable<GestureEdit> GestureEditsObservable
	{
		get => GetValue(GestureEditsObservableProperty);
		set => SetValue(GestureEditsObservableProperty, value);
	}

	public string? Watermark
	{
		get => GetValue(WatermarkProperty);
		set => SetValue(WatermarkProperty, value);
	}

	public GestureBox()
	{
		GestureEditsObservable = new InputElementGestureObservable(this);
		PointerReleased += OnPointerClick;
	}

	protected override void OnLostFocus(RoutedEventArgs args)
	{
		if (IsEditing)
			StopEditing();
	}

	private bool IsEmpty
	{
		set
		{
			if (value == field)
				return;
			field = value;
			PseudoClasses.Set(EmptyPseudoClass, value);
		}
	}

	private bool IsEditing
	{
		get;
		set
		{
			field = value;
			PseudoClasses.Set(EditingPseudoClass, value);
		}
	}

	private IDisposable _editingDisposable = Disposable.Empty;

	private void OnPointerClick(object? sender, PointerReleasedEventArgs args)
	{
		if (args.InitialPressMouseButton == MouseButton.Left)
			StartEditing();
		else if (args.InitialPressMouseButton == MouseButton.Right)
			ClearGesture();
	}

	private void StartEditing()
	{
		ClearGesture();
		IsEditing = true;
		Focus();
		PointerReleased -= OnPointerClick;
		_editingDisposable = GestureEditsObservable.ObserveOn(SynchronizationContext.Current).Subscribe(SetGesture);
	}

	private void SetGesture(GestureEdit edit)
	{
		SetCurrentValue(GestureProperty, edit.Gesture);
		IsEmpty = edit.Gesture == null;
		if (edit.ShouldStopEditing)
			StopEditing();
	}

	private void ClearGesture()
	{
		SetCurrentValue(GestureProperty, null);
		IsEmpty = true;
	}

	private void StopEditing()
	{
		IsEditing = false;
		PointerReleased += OnPointerClick;
		_editingDisposable.Dispose();
	}
}