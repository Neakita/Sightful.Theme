using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia.Theme.ThemeVariants;

internal abstract class ThemeColorsDictionary : ResourceDictionary
{
	protected abstract Color InnerBackgroundColor { get; }
	protected abstract Color OuterBackgroundColor { get; }
	protected abstract Color ControlBackgroundColor { get; }
	protected abstract Color PrimaryTextColor { get; }
	protected abstract Color SecondaryTextColor { get; }
	protected abstract Color WatermarkPrimaryColor { get; }
	protected abstract Color WatermarkSecondaryColor { get; }
	protected abstract Color ButtonHoverBackgroundColor { get; }
	protected abstract Color PressedButtonBackgroundColor { get; }
	protected abstract Color ToolTipTextColor { get; }
	protected abstract Color LowAccentButtonBackgroundColor { get; }
	protected abstract Color LowAccentHoveredButtonBackgroundColor { get; }
	protected abstract Color LowAccentPressedButtonBackgroundColor { get; }
	protected abstract Color AccentColor { get; }
	protected abstract Color HighAccentButtonBackgroundColor { get; }
	protected abstract Color HighAccentHoveredButtonBackgroundColor { get; }
	protected abstract Color HighAccentButtonForegroundColor { get; }

	protected ThemeColorsDictionary()
	{
		AddResources();
	}

	private void AddResources()
	{
		AddColorAndBrush(InnerBackgroundColor);
		AddColorAndBrush(OuterBackgroundColor);
		AddColorAndBrush(ControlBackgroundColor);
		AddColorAndBrush(PrimaryTextColor);
		AddColorAndBrush(SecondaryTextColor);
		AddColorAndBrush(WatermarkPrimaryColor);
		AddColorAndBrush(WatermarkSecondaryColor);
		AddColorAndBrush(ButtonHoverBackgroundColor);
		AddColorAndBrush(PressedButtonBackgroundColor);
		AddColorAndBrush(ToolTipTextColor);
		AddColorAndBrush(LowAccentButtonBackgroundColor);
		AddColorAndBrush(LowAccentHoveredButtonBackgroundColor);
		AddColorAndBrush(LowAccentPressedButtonBackgroundColor);
		AddColorAndBrush(AccentColor);
		AddColorAndBrush(HighAccentButtonBackgroundColor);
		AddColorAndBrush(HighAccentHoveredButtonBackgroundColor);
		AddColorAndBrush(HighAccentButtonForegroundColor);
	}

	private void AddColorAndBrush(Color color, [CallerArgumentExpression(nameof(color))] string key = "")
	{
		AddColor(color, key);
		AddBrush(color, key);
	}
	
	private void AddColor(Color color, string key)
	{
		Add(key, color);
	}

	private void AddBrush(Color color, string key)
	{
		const string postfixToReplace = "Color";
		Guard.IsTrue(key.EndsWith(postfixToReplace));
		var brushKey = key[..^postfixToReplace.Length] + "Brush";
		SolidColorBrush brush = new(color);
		Add(brushKey, brush);
	}
}