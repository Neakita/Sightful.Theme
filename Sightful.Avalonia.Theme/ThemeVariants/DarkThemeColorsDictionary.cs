using Avalonia.Media;

namespace Sightful.Avalonia.Theme.ThemeVariants;

internal sealed class DarkThemeColorsDictionary : ThemeColorsDictionary
{
	protected override Color InnerBackground => Color.FromRgb(0x16, 0x18, 0x1C);
	protected override Color OuterBackground => Color.FromRgb(0x26, 0x29, 0x2F);
	protected override Color ControlBackground => Color.FromRgb(0x43, 0x49, 0x56);
	protected override Color PrimaryForeground => Color.FromRgb(0xEC, 0xF9, 0xFB);
	protected override Color SecondaryForeground => Color.FromRgb(0xB0, 0xBA, 0xC5);
	protected override Color WatermarkPrimary => Color.FromRgb(0x9E, 0xA8, 0xB3);
	protected override Color WatermarkSecondary => Color.FromRgb(0x85, 0x8D, 0x98);
	protected override Color ButtonHoverBackground => Color.FromRgb(0x39, 0x3E, 0x49);
	protected override Color PressedButtonBackground => Color.FromRgb(0x36, 0x3A, 0x45);
	protected override Color ToolTipForeground => Color.FromRgb(0xAB, 0xB4, 0xC0);
	protected override Color LowAccentButtonBackground => Color.FromRgb(0x23, 0x55, 0x3E);
	protected override Color LowAccentHoveredButtonBackground => Color.FromRgb(0x22, 0x4D, 0x3A);
	protected override Color LowAccentPressedButtonBackground => Color.FromRgb(0x22, 0x4A, 0x39);
	protected override Color Accent => Color.FromRgb(0x1E, 0xA4, 0x58);
	protected override Color HighAccentButtonBackground => Color.FromRgb(0x1B, 0xD9, 0x6A);
	protected override Color HighAccentHoveredButtonBackground => Color.FromRgb(0x17, 0xB8, 0x5A);
	protected override Color HighAccentPressedButtonBackground => Color.FromRgb(0x16, 0xAE, 0x55);
	protected override Color HighAccentButtonForeground => Colors.Black;
}