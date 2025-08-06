using System.Collections.Frozen;
using System.Reactive.Disposables;
using Avalonia.Input;

namespace Sightful.Avalonia.Controls.GestureBox;

internal sealed class InputElementGestureObservable : IObservable<GestureEdit>
{
	public InputElementGestureObservable(InputElement inputElement)
	{
		_inputElement = inputElement;
	}

	public IDisposable Subscribe(IObserver<GestureEdit> observer)
	{
		_observers.Add(observer);
		if (_observers.Count == 1)
			SubscribeToEvents();
		return Disposable.Create(observer, Unsubscribe);
	}


	private static readonly FrozenDictionary<Key, KeyModifiers> KeyModifiersMap =
		FrozenDictionary.ToFrozenDictionary<Key, KeyModifiers>([
			new(Key.LeftAlt, KeyModifiers.Alt),
			new(Key.RightAlt, KeyModifiers.Alt),
			new(Key.LeftCtrl, KeyModifiers.Control),
			new(Key.RightCtrl, KeyModifiers.Control),
			new(Key.LeftShift, KeyModifiers.Shift),
			new(Key.RightShift, KeyModifiers.Shift),
			new(Key.LWin, KeyModifiers.Meta),
			new(Key.RWin, KeyModifiers.Meta)
		]);

	private readonly InputElement _inputElement;
	private readonly List<IObserver<GestureEdit>> _observers = new();

	private void SubscribeToEvents()
	{
		_inputElement.PointerReleased += OnPointerReleased;
		_inputElement.KeyUp += OnKeyUp;
	}

	private void Unsubscribe(IObserver<GestureEdit> observer)
	{
		_observers.Remove(observer);
		if (_observers.Count <= 0)
			UnsubscribeFromEvents();
	}

	private void UnsubscribeFromEvents()
	{
		_inputElement.PointerReleased -= OnPointerReleased;
		_inputElement.KeyUp -= OnKeyUp;
	}

	private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		var gesture = new MouseButtonGesture(e.InitialPressMouseButton, e.KeyModifiers);
		var edit = new GestureEdit(gesture, true);
		foreach (var observer in _observers)
			observer.OnNext(edit);
	}

	private void OnKeyUp(object? sender, KeyEventArgs e)
	{
		var key = e.Key;
		var modifiers = e.KeyModifiers;
		// for some reason when any modifier key (for example shift) is pressed and released args contains both Key.LShift and KeyModifiers.Shift
		modifiers = RemoveMatchingModifier(key, modifiers);
		var gesture = new KeyGesture(key, modifiers);
		var edit = new GestureEdit(gesture, true);
		foreach (var observer in _observers)
			observer.OnNext(edit);
	}

	private static KeyModifiers RemoveMatchingModifier(Key key, KeyModifiers modifiers)
	{
		if (KeyModifiersMap.TryGetValue(key, out var modifierToRemove))
			modifiers &= ~modifierToRemove;
		return modifiers;
	}
}