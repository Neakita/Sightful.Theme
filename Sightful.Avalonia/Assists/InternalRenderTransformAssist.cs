using Avalonia;
using Avalonia.Layout;
using Avalonia.Media.Transformation;
using Avalonia.Reactive;

namespace Sightful.Avalonia.Assists;

internal static class InternalRenderTransformAssist
{
	static InternalRenderTransformAssist()
	{
		ShrinkingProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<double>>(OnShrinkingPropertyChanged));
		RotationProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<double>>(OnRotationPropertyChanged));
	}

	#region Shrinking

	public static readonly AvaloniaProperty<double> ShrinkingProperty =
		AvaloniaProperty.RegisterAttached<Layoutable, double>("Shrinking", typeof(InternalRenderTransformAssist));

	public static double GetShrinking(Layoutable element)
	{
		return element.GetValue<double>(ShrinkingProperty);
	}

	public static void SetShrinking(Layoutable element, double value)
	{
		element.SetValue(ShrinkingProperty, value);
	}

	private static void OnShrinkingPropertyChanged(AvaloniaPropertyChangedEventArgs<double> args)
	{
		var sender = (Layoutable)args.Sender;
		var originalSize = sender.DesiredSize;
		var originalSizeLowerDimensional = Math.Min(originalSize.Width, originalSize.Height);
		var targetSize = originalSizeLowerDimensional - args.NewValue.Value;
		var scale = targetSize / originalSizeLowerDimensional;
		var builder = TransformOperations.CreateBuilder(1);
		builder.AppendScale(scale, scale);
		sender.RenderTransform = builder.Build();
	}

	#endregion

	#region Rotation

	public static readonly AttachedProperty<double> RotationProperty =
		AvaloniaProperty.RegisterAttached<Visual, double>("Rotation", typeof(InternalRenderTransformAssist));

	public static double GetRotation(Visual element)
	{
		return element.GetValue(RotationProperty);
	}

	public static void SetRotation(Visual element, double value)
	{
		element.SetValue(RotationProperty, value);
	}

	private static void OnRotationPropertyChanged(AvaloniaPropertyChangedEventArgs<double> args)
	{
		var sender = (Visual)args.Sender;
		var angle = RadiansToDegrees(args.NewValue.Value);
		var builder = TransformOperations.CreateBuilder(1);
		builder.AppendRotate(angle);
		sender.RenderTransform = builder.Build();
	}

	private static double RadiansToDegrees(double radians)
	{
		return radians * Math.PI / 180;
	}

	#endregion
}