using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media.Transformation;
using Avalonia.Reactive;

namespace Sightful.Avalonia.Assists;

internal static class InternalShrinkingAssist
{
	static InternalShrinkingAssist()
	{
		ShrinkingProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<double>>(OnNext));
	}

	private static void OnNext(AvaloniaPropertyChangedEventArgs<double> args)
	{
		if (args.Sender is not Layoutable layoutable)
			return;
		var originalSize = layoutable.DesiredSize;
		var originalSizeLowerDimensional = Math.Min(originalSize.Width, originalSize.Height);
		var targetSize = originalSizeLowerDimensional - args.NewValue.Value;
		var scale = targetSize / originalSizeLowerDimensional;
		var builder = TransformOperations.CreateBuilder(1);
		builder.AppendScale(scale, scale);
		layoutable.RenderTransform = builder.Build();
	}

	public static readonly AvaloniaProperty<double> ShrinkingProperty =
		AvaloniaProperty.RegisterAttached<Control, double>("Shrinking", typeof(InternalShrinkingAssist));

	public static double GetShrinking(AvaloniaObject element)
	{
		return element.GetValue<double>(ShrinkingProperty);
	}

	public static void SetShrinking(AvaloniaObject element, double value)
	{
		element.SetValue(ShrinkingProperty, value);
	}
}