using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Sightful.Avalonia.Controls.GestureBox;

[PseudoClasses(EmptyPseudoClass, EditingPseudoClass)]
public sealed class GestureBox : TemplatedControl
{
	public static readonly StyledProperty<object?> GestureProperty =
		AvaloniaProperty.Register<GestureBox, object?>(nameof(Gesture));

	public static readonly StyledProperty<IObservable<object?>> GestureObservableProperty =
		AvaloniaProperty.Register<GestureBox, IObservable<object?>>(nameof(GestureObservable));

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

	public IObservable<object?> GestureObservable
	{
		get => GetValue(GestureObservableProperty);
		set => SetValue(GestureObservableProperty, value);
	}

	public GestureBox()
	{
		GestureObservable = new InputElementGestureObservable(this);
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
		IsEditing = true;
		Focus();
		PointerReleased -= OnPointerClick;
		_editingDisposable = GestureObservable.ObserveOn(SynchronizationContext.Current).Subscribe(SetGesture);
	}

	private void SetGesture(object? gesture)
	{
		SetCurrentValue(GestureProperty, gesture);
		IsEmpty = gesture == null;
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