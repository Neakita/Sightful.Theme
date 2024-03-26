using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Media;

namespace Sightful.Avalonia.Theme.ThemeVariants;

internal abstract class ThemeColorsDictionary : ResourceDictionary
{
	protected abstract Color InnerBackground { get; }
	protected abstract Color OuterBackground { get; }
	protected abstract Color ControlBackground { get; }
	protected abstract Color PrimaryForeground { get; }
	protected abstract Color SecondaryForeground { get; }
	protected abstract Color WatermarkPrimary { get; }
	protected abstract Color WatermarkSecondary { get; }
	protected abstract Color ButtonHoverBackground { get; }
	protected abstract Color PressedButtonBackground { get; }
	protected abstract Color ToolTipForeground { get; }
	protected abstract Color LowAccentButtonBackground { get; }
	protected abstract Color LowAccentHoveredButtonBackground { get; }
	protected abstract Color LowAccentPressedButtonBackground { get; }
	protected abstract Color Accent { get; }
	protected abstract Color HighAccentButtonBackground { get; }
	protected abstract Color HighAccentHoveredButtonBackground { get; }
	protected abstract Color HighAccentButtonForeground { get; }

	protected ThemeColorsDictionary()
	{
		AddResources();
	}

	private void AddResources()
	{
		AddColorAndBrush(InnerBackground);
		AddColorAndBrush(OuterBackground);
		AddColorAndBrush(ControlBackground);
		AddColorAndBrush(PrimaryForeground);
		AddColorAndBrush(SecondaryForeground);
		AddColorAndBrush(WatermarkPrimary);
		AddColorAndBrush(WatermarkSecondary);
		AddColorAndBrush(ButtonHoverBackground);
		AddColorAndBrush(PressedButtonBackground);
		AddColorAndBrush(ToolTipForeground);
		AddColorAndBrush(LowAccentButtonBackground);
		AddColorAndBrush(LowAccentHoveredButtonBackground);
		AddColorAndBrush(LowAccentPressedButtonBackground);
		AddColorAndBrush(Accent);
		AddColorAndBrush(HighAccentButtonBackground);
		AddColorAndBrush(HighAccentHoveredButtonBackground);
		AddColorAndBrush(HighAccentButtonForeground);
	}

	private void AddColorAndBrush(Color color, [CallerArgumentExpression(nameof(color))] string key = "")
	{
		AddColor(color, key);
	}
	
	private void AddColor(Color color, string key)
	{
		Add(key, color);
	}
}