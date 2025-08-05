using System.Collections.Frozen;
using System.Reactive.Disposables;
using Avalonia.Input;

namespace Sightful.Avalonia.Controls.GestureBox;

internal sealed class InputElementGestureObservable : IObservable<object?>
{
	public InputElementGestureObservable(InputElement inputElement)
	{
		_inputElement = inputElement;
	}

	public IDisposable Subscribe(IObserver<object?> observer)
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
	private readonly List<IObserver<object?>> _observers = new();

	private void SubscribeToEvents()
	{
		_inputElement.PointerReleased += OnPointerClickWhileEditing;
		_inputElement.KeyUp += OnKeyClickWhileEditing;
	}

	private void Unsubscribe(IObserver<object?> observer)
	{
		_observers.Remove(observer);
		if (_observers.Count <= 0)
			UnsubscribeFromEvents();
	}

	private void UnsubscribeFromEvents()
	{
		_inputElement.PointerReleased -= OnPointerClickWhileEditing;
		_inputElement.KeyUp -= OnKeyClickWhileEditing;
	}

	private void OnPointerClickWhileEditing(object? sender, PointerReleasedEventArgs e)
	{
		var gesture = new MouseButtonGesture(e.InitialPressMouseButton, e.KeyModifiers);
		foreach (var observer in _observers)
			observer.OnNext(gesture);
	}

	private void OnKeyClickWhileEditing(object? sender, KeyEventArgs e)
	{
		var key = e.Key;
		var modifiers = e.KeyModifiers;
		// for some reason when any modifier key (for example shift) is pressed and released args contains both Key.LShift and KeyModifiers.Shift
		modifiers = RemoveMatchingModifier(key, modifiers);
		KeyGesture gesture = new(key, modifiers);
		foreach (var observer in _observers)
			observer.OnNext(gesture);
	}

	private static KeyModifiers RemoveMatchingModifier(Key key, KeyModifiers modifiers)
	{
		if (KeyModifiersMap.TryGetValue(key, out var modifierToRemove))
			modifiers &= ~modifierToRemove;
		return modifiers;
	}
}