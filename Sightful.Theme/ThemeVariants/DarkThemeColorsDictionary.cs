using Avalonia.Media;

namespace Sightful.Theme.ThemeVariants;

internal sealed class DarkThemeColorsDictionary : ThemeColorsDictionary
{
	protected override Color InnerBackgroundColor => Color.FromRgb(0x16, 0x18, 0x1C);
	protected override Color OuterBackgroundColor => Color.FromRgb(0x26, 0x29, 0x2F);
	protected override Color ControlBackgroundColor => Color.FromRgb(0x43, 0x49, 0x56);
	protected override Color PrimaryTextColor => Color.FromRgb(0xEC, 0xF9, 0xFB);
	protected override Color SecondaryTextColor => Color.FromRgb(0xB0, 0xBA, 0xC5);
	protected override Color WatermarkPrimaryColor => Color.FromRgb(0x9E, 0xA8, 0xB3);
	protected override Color WatermarkSecondaryColor => Color.FromRgb(0x85, 0x8D, 0x98);
	protected override Color ButtonHoverBackgroundColor => Color.FromRgb(0x39, 0x3E, 0x49);
	protected override Color PressedButtonBackgroundColor => Color.FromRgb(0x36, 0x3A, 0x45);
	protected override Color ToolTipTextColor => Color.FromRgb(0xAB, 0xB4, 0xC0);
	protected override Color LowAccentButtonBackgroundColor => Color.FromRgb(0x23, 0x55, 0x3E);
	protected override Color LowAccentHoveredButtonBackgroundColor => Color.FromRgb(0x22, 0x4D, 0x3A);
	protected override Color LowAccentPressedButtonBackgroundColor => Color.FromRgb(0x22, 0x4A, 0x39);
	protected override Color AccentColor => Color.FromRgb(0x1E, 0xA4, 0x58);
	protected override Color HighAccentButtonBackgroundColor => Color.FromRgb(0x1B, 0xD9, 0x6A);
	protected override Color HighAccentHoveredButtonBackgroundColor => Color.FromRgb(0x17, 0xB8, 0x5A);
	protected override Color HighAccentButtonForegroundColor => Colors.Black;
}