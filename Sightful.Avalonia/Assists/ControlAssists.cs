using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Reactive;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia.Assists;

internal static class ControlAssists
{
	static ControlAssists()
	{
		TrackLoadingProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnTrackLoadingChanged));
	}

	private static void OnTrackLoadingChanged(AvaloniaPropertyChangedEventArgs<bool> args)
	{
		var control = (Control)args.Sender;
		bool shouldTrack = args.NewValue.Value;
		if (shouldTrack)
		{
			control.Loaded += OnControlLoaded;
			control.DetachedFromVisualTree += OnControlDetached;
		}
		else
		{
			control.Loaded -= OnControlLoaded;
			control.DetachedFromVisualTree -= OnControlDetached;
		}
	}

	private static void OnControlLoaded(object? sender, RoutedEventArgs e)
	{
		Guard.IsNotNull(sender);
		var control = (Control)sender;
		SetIsLoaded(control, true);
	}

	private static void OnControlDetached(object? sender, VisualTreeAttachmentEventArgs e)
	{
		Guard.IsNotNull(sender);
		var control = (Control)sender;
		SetIsLoaded(control, false);
	}

	public static readonly AttachedProperty<bool> TrackLoadingProperty =
		AvaloniaProperty.RegisterAttached<Control, bool>("TrackLoading", typeof(ControlAssists));

	public static bool GetTrackLoading(AvaloniaObject element)
	{
		return element.GetValue(TrackLoadingProperty);
	}

	public static void SetTrackLoading(AvaloniaObject element, bool value)
	{
		element.SetValue(TrackLoadingProperty, value);
	}

	public static readonly AttachedProperty<bool> IsLoadedProperty =
		AvaloniaProperty.RegisterAttached<Control, bool>("IsLoaded", typeof(ControlAssists));

	public static bool GetIsLoaded(AvaloniaObject element)
	{
		return element.GetValue(IsLoadedProperty);
	}

	public static void SetIsLoaded(AvaloniaObject element, bool value)
	{
		element.SetValue(IsLoadedProperty, value);
	}
}