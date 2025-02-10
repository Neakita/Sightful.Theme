using System.Collections.Frozen;
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

	static GestureBox()
	{
		FocusableProperty.OverrideDefaultValue<GestureBox>(true);
	}

	private const string EmptyPseudoClass = ":empty";
	private const string EditingPseudoClass = ":editing";

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

	public object? Gesture
	{
		get => GetValue(GestureProperty);
		set => SetValue(GestureProperty, value);
	}

	public GestureBox()
	{
		PointerReleased += OnPointerClick;
	}

	protected override void OnLostFocus(RoutedEventArgs args)
	{
		if (IsEditing)
			StopEditing();
	}

	private bool IsEmpty
	{
		get;
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
		PointerReleased += OnPointerClickWhileEditing;
		KeyUp += OnKeyClickWhileEditing;
	}

	private void OnPointerClickWhileEditing(object? sender, PointerReleasedEventArgs args)
	{
		var button = args.InitialPressMouseButton;
		MouseButtonGesture gesture = new(button, args.KeyModifiers);
		SetGesture(gesture);
		StopEditing();
	}

	private void OnKeyClickWhileEditing(object? sender, KeyEventArgs args)
	{
		var key = args.Key;
		var modifiers = args.KeyModifiers;
		// for some reason when any modifier key (for example shift) is pressed and released args contains both Key.LShift and KeyModifiers.Shift
		modifiers = RemoveMatchingModifier(key, modifiers);
		KeyGesture gesture = new(key, modifiers);
		SetGesture(gesture);
		StopEditing();
	}

	private static KeyModifiers RemoveMatchingModifier(Key key, KeyModifiers modifiers)
	{
		if (KeyModifiersMap.TryGetValue(key, out var modifierToRemove))
			modifiers &= ~modifierToRemove;
		return modifiers;
	}

	private void SetGesture(object gesture)
	{
		SetCurrentValue(GestureProperty, gesture);
		IsEmpty = false;
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
		PointerReleased -= OnPointerClickWhileEditing;
		KeyUp -= OnKeyClickWhileEditing;
	}
}