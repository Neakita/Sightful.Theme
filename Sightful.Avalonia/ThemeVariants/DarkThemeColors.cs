﻿using Avalonia.Media;

namespace Sightful.Avalonia.ThemeVariants;

internal sealed class DarkThemeColors : ThemeColors
{
	protected override Color InnerBackground => Color.FromRgb(0x16, 0x18, 0x1C);
	protected override Color OuterBackground => Color.FromRgb(0x26, 0x29, 0x2F);
	protected override Color ControlBackground => Color.FromRgb(0x43, 0x49, 0x56);
	protected override Color PrimaryForeground => Color.FromRgb(0xEC, 0xF9, 0xFB);
	protected override Color SecondaryForeground => Color.FromRgb(0xB0, 0xBA, 0xC5);
	protected override Color WatermarkPrimary => Color.FromRgb(0x9E, 0xA8, 0xB3);
	protected override Color WatermarkSecondary => Color.FromRgb(0x85, 0x8D, 0x98);
	protected override Color ButtonHoverBackground => Color.FromRgb(0x39, 0x3E, 0x49);
	protected override Color DisabledButtonBackground => Color.FromRgb(0x2E, 0x31, 0x36);
	protected override Color PressedButtonBackground => Color.FromRgb(0x36, 0x3A, 0x45);
	protected override Color ToolTipForeground => Color.FromRgb(0xAB, 0xB4, 0xC0);
	protected override Color LowAccentButtonBackground => Color.FromRgb(0x23, 0x55, 0x3E);
	protected override Color LowAccentHoveredButtonBackground => Color.FromRgb(0x22, 0x4D, 0x3A);
	protected override Color LowAccentPressedButtonBackground => Color.FromRgb(0x22, 0x4A, 0x39);
	protected override Color Accent => Color.FromRgb(0x1E, 0xA4, 0x58);
	protected override Color HighAccentButtonBackground => Color.FromRgb(0x1B, 0xD9, 0x6A);
	protected override Color HighAccentHoveredButtonBackground => Color.FromRgb(0x17, 0xB8, 0x5A);
	protected override Color HighAccentPressedButtonBackground => Color.FromRgb(0x16, 0xAE, 0x55);
	protected override Color HighAccentDisabledButtonBackground => Color.FromRgb(0x44, 0x75, 0x5C);
	protected override Color CloseWindowHoveredButtonBackground => Color.FromRgb(0xD9, 0x3E, 0x5D);
	protected override Color CloseWindowPressedButtonBackground => Color.FromRgb(0xCC, 0x3A, 0x58);
	protected override Color HighAccentButtonForeground => Colors.Black;
	protected override Color TextSelection => Color.FromRgb(0x0D, 0x43, 0xAF);
	protected override Color DisabledTextBoxBackground => Color.FromRgb(0x37, 0x3C, 0x47);
	protected override Color ToggleSwitchKnob => Color.FromRgb(0x9F, 0xA4, 0xB3);
	protected override Color HoveredToggleSwitchKnob => Color.FromRgb(0x87, 0x8B, 0x98);
	protected override Color PressedToggleSwitchKnob => Color.FromRgb(0x7F, 0x83, 0x8F);
	protected override Color HoveredComboBoxItemForeground => Color.FromRgb(0x96, 0x9E, 0xA7);

	public DarkThemeColors()
	{
		SetColors();
	}
}