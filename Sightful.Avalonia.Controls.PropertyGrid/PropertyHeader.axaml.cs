using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Sightful.Avalonia.Controls.PropertyGrid;

public class PropertyHeader : TemplatedControl
{
	public static readonly StyledProperty<string> PropertyNameProperty =
		AvaloniaProperty.Register<PropertyHeader, string>(nameof(PropertyName), string.Empty);

	public static readonly StyledProperty<string> PropertyDescriptionProperty =
		AvaloniaProperty.Register<PropertyHeader, string>(nameof(PropertyDescription), string.Empty);

	public string PropertyName
	{
		get => GetValue(PropertyNameProperty);
		set => SetValue(PropertyNameProperty, value);
	}

	public string PropertyDescription
	{
		get => GetValue(PropertyDescriptionProperty);
		set => SetValue(PropertyDescriptionProperty, value);
	}
}