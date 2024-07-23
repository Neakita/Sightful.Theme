using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CommunityToolkit.Diagnostics;

namespace Sightful.Avalonia.ThemeVariants;

internal class ThemeColors : ResourceDictionary
{
	protected virtual Color InnerBackground => Colors.Black;
	protected virtual Color OuterBackground => Colors.Black;
	protected virtual Color ControlBackground => Colors.Black;
	protected virtual Color PrimaryForeground => Colors.Black;
	protected virtual Color SecondaryForeground => Colors.Black;
	protected virtual Color WatermarkPrimary => Colors.Black;
	protected virtual Color WatermarkSecondary => Colors.Black;
	protected virtual Color ButtonHoverBackground => Colors.Black;
	protected virtual Color PressedButtonBackground => Colors.Black;
	protected virtual Color ToolTipForeground => Colors.Black;
	protected virtual Color LowAccentButtonBackground => Colors.Black;
	protected virtual Color LowAccentHoveredButtonBackground => Colors.Black;
	protected virtual Color LowAccentPressedButtonBackground => Colors.Black;
	protected virtual Color Accent => Colors.Black;
	protected virtual Color HighAccentButtonBackground => Colors.Black;
	protected virtual Color HighAccentHoveredButtonBackground => Colors.Black;
	protected virtual Color HighAccentButtonForeground => Colors.Black;
	protected virtual Color HighAccentPressedButtonBackground => Colors.Black;
	protected virtual Color CloseWindowHoveredButtonBackground => Colors.Black;
	protected virtual Color CloseWindowPressedButtonBackground => Colors.Black;
	protected virtual Color TextSelection => Colors.Black;
	protected virtual Color DisabledButtonBackground => Colors.Black;
	protected virtual Color HighAccentDisabledButtonBackground => Colors.Black;
	protected virtual Color DisabledTextBoxBackground => Colors.Black;
	protected virtual Color ToggleSwitchKnob => Colors.Black;
	protected virtual Color HoveredToggleSwitchKnob => Colors.Black;
	protected virtual Color PressedToggleSwitchKnob => Colors.Black;

	public ThemeColors()
	{
		AvaloniaXamlLoader.Load(this);
	}

	protected void SetColors()
	{
		SetColor(InnerBackground);
		SetColor(OuterBackground);
		SetColor(ControlBackground);
		SetColor(PrimaryForeground);
		SetColor(SecondaryForeground);
		SetColor(WatermarkPrimary);
		SetColor(WatermarkSecondary);
		SetColor(ButtonHoverBackground);
		SetColor(PressedButtonBackground);
		SetColor(ToolTipForeground);
		SetColor(LowAccentButtonBackground);
		SetColor(LowAccentHoveredButtonBackground);
		SetColor(LowAccentPressedButtonBackground);
		SetColor(Accent);
		SetColor(HighAccentButtonBackground);
		SetColor(HighAccentHoveredButtonBackground);
		SetColor(HighAccentButtonForeground);
		SetColor(HighAccentPressedButtonBackground);
		SetColor(CloseWindowHoveredButtonBackground);
		SetColor(CloseWindowPressedButtonBackground);
		SetColor(TextSelection);
		SetColor(DisabledButtonBackground);
		SetColor(HighAccentDisabledButtonBackground);
		SetColor(DisabledTextBoxBackground);
		SetColor(ToggleSwitchKnob);
		SetColor(HoveredToggleSwitchKnob);
		SetColor(PressedToggleSwitchKnob);
	}

	private void SetColor(Color color, [CallerArgumentExpression(nameof(color))] string key = "")
	{
		Guard.IsTrue(ContainsKey(key));
		this[key] = new SolidColorBrush(color);
	}
}