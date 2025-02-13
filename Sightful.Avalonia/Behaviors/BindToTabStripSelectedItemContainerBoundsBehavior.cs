using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Xaml.Interactivity;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia.Behaviors;

internal sealed class BindToTabStripSelectedItemContainerBoundsBehavior : Behavior<Border>
{
	public static readonly StyledProperty<TabStrip?> TabStripProperty =
		AvaloniaProperty.Register<BindToTabStripSelectedItemContainerBoundsBehavior, TabStrip?>(nameof(TabStrip));

	public TabStrip? TabStrip
	{
		get => GetValue(TabStripProperty);
		set => SetValue(TabStripProperty, value);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == TabStripProperty)
			UpdateBindings();
	}

	private void UpdateBindings()
	{
		if (TabStrip == null || AssociatedObject == null)
			return;
		var boundsObservable = TabStrip
			.GetObservable(SelectingItemsControl.SelectedIndexProperty)
			.Select(BoundsFromIndex)
			.Switch()
			.Publish()
			.RefCount();
		AssociatedObject.Bind(Layoutable.WidthProperty,
			boundsObservable.Select(bounds => bounds.Width));
		AssociatedObject.Bind(Canvas.LeftProperty,
			boundsObservable.Select(bounds => bounds.X));
	}

	private IObservable<Rect> BoundsFromIndex(int index)
	{
		Guard.IsNotNull(TabStrip);
		var container = TabStrip.ContainerFromIndex(index);
		if (container == null)
			return Observable.Empty<Rect>();
		return container.GetObservable(Visual.BoundsProperty);
	}
}