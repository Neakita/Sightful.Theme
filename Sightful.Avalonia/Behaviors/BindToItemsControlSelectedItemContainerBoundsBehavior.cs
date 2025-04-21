using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Xaml.Interactivity;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia.Behaviors;

internal sealed class BindToItemsControlSelectedItemContainerBoundsBehavior : Behavior<Border>
{
	public static readonly StyledProperty<SelectingItemsControl?> ItemsControlProperty =
		AvaloniaProperty.Register<BindToItemsControlSelectedItemContainerBoundsBehavior, SelectingItemsControl?>(nameof(ItemsControl));

	public SelectingItemsControl? ItemsControl
	{
		get => GetValue(ItemsControlProperty);
		set => SetValue(ItemsControlProperty, value);
	}

	protected override void OnAttached()
	{
		Guard.IsNotNull(AssociatedObject);
		AssociatedObject.Loaded += OnAssociatedObjectLoaded;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ItemsControlProperty)
			UpdateBindings();
	}

	private void OnAssociatedObjectLoaded(object? sender, RoutedEventArgs e)
	{
		Guard.IsNotNull(AssociatedObject);
		AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
		UpdateBindings();
	}

	private void UpdateBindings()
	{
		if (ItemsControl == null || AssociatedObject == null)
			return;
		var boundsObservable = ItemsControl
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
		Guard.IsNotNull(ItemsControl);
		var container = ItemsControl.ContainerFromIndex(index);
		if (container == null)
			return Observable.Empty<Rect>();
		return container.GetObservable(Visual.BoundsProperty);
	}
}