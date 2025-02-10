using System.Text;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace Sightful.Avalonia.Controls.GestureBox;

public sealed class MouseButtonGesture
{
	public MouseButton Button { get; }
	public KeyModifiers Modifiers { get; }

	public MouseButtonGesture(MouseButton button, KeyModifiers modifiers)
	{
		Button = button;
		Modifiers = modifiers;
	}

	public override string ToString() => ToString(null, null);

	/// <summary>
	/// Returns the current KeyGesture as a string formatted according to the format string and appropriate IFormatProvider
	/// </summary>
	/// <param name="format">The format to use. 
	/// <list type="bullet">
	/// <item><term>null, "" or "g"</term><description>The Invariant format, uses Enum.ToString() to format Keys.</description></item>
	/// <item><term>"p"</term><description>Use platform specific formatting as registered.</description></item>
	/// </list></param>
	/// <param name="formatProvider">The IFormatProvider to use. If null, uses the appropriate provider registered in the Avalonia Locator, or Invariant.</param>
	/// <returns>The formatted string.</returns>
	/// <exception cref="FormatException">Thrown if the format string is not null, "", "g", or "p"</exception>
	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		var formatInfo = format switch
		{
			null or "" or "g" => KeyGestureFormatInfo.Invariant,
			"p" => KeyGestureFormatInfo.GetInstance(formatProvider),
			_ => throw new FormatException("Unknown format specifier")
		};

		StringBuilder stringBuilder = new();

		if (HasFlag(Modifiers, KeyModifiers.Control))
		{
			stringBuilder.Append(formatInfo.Ctrl);
		}

		if (HasFlag(Modifiers, KeyModifiers.Shift))
		{
			Plus(stringBuilder);
			stringBuilder.Append(formatInfo.Shift);
		}

		if (HasFlag(Modifiers, KeyModifiers.Alt))
		{
			Plus(stringBuilder);
			stringBuilder.Append(formatInfo.Alt);
		}

		if (HasFlag(Modifiers, KeyModifiers.Meta))
		{
			Plus(stringBuilder);
			stringBuilder.Append(formatInfo.Meta);
		}

		Plus(stringBuilder);
		stringBuilder.Append(Button.ToString());

		return stringBuilder.ToString();

		static void Plus(StringBuilder s)
		{
			if (s.Length > 0)
			{
				s.Append('+');
			}
		}
	}

	private static bool HasFlag(KeyModifiers value, KeyModifiers flag)
	{
		return (value & flag) != 0;
	}
}