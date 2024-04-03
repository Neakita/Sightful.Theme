using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Sightful.Avalonia.Theme;

public sealed class DefaultTheme : Styles, IResourceNode
{
	public static readonly DirectProperty<DefaultTheme, AppearanceTheme> AppearanceThemeProperty = AvaloniaProperty.RegisterDirect<DefaultTheme, AppearanceTheme>(
		nameof(AppearanceTheme), o => o.AppearanceTheme, (o, v) => o.AppearanceTheme = v);
	public static readonly DirectProperty<DefaultTheme, SpatialTheme> SpatialThemeProperty = AvaloniaProperty.RegisterDirect<DefaultTheme, SpatialTheme>(
		nameof(SpatialTheme), o => o.SpatialTheme, (o, v) => o.SpatialTheme = v);

	public AppearanceTheme AppearanceTheme
	{
		get => _appearanceThemeTheme;
		set
		{
			if (SetAndRaise(AppearanceThemeProperty, ref _appearanceThemeTheme, value))
				value.Initialize();
		}
	}

	public SpatialTheme SpatialTheme
	{
		get => _spatialTheme;
		set
		{
			if (SetAndRaise(SpatialThemeProperty, ref _spatialTheme, value))
				value.Initialize();
		}
	}

	public DefaultTheme(IServiceProvider? serviceProvider = null)
	{
		_appearanceThemeTheme = new AppearanceTheme();
		_appearanceThemeTheme.Initialize();
		_spatialTheme = new SpatialTheme();
		_spatialTheme.Initialize();
		AvaloniaXamlLoader.Load(serviceProvider, this);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == AppearanceThemeProperty)
			Owner?.NotifyHostedResourcesChanged(ResourcesChangedEventArgs.Empty);
	}

	bool IResourceNode.TryGetResource(object key, ThemeVariant? theme, out object? value)
	{
		return AppearanceTheme.TryGetResource(key, theme, out value) || SpatialTheme.TryGetResource(key, theme, out value) || base.TryGetResource(key, theme, out value);
	}

	private AppearanceTheme _appearanceThemeTheme;
	private SpatialTheme _spatialTheme;
}