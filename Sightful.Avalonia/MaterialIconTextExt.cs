using Material.Icons;

namespace Sightful.Avalonia;

public sealed class MaterialIconTextExt
{
	public double? Spacing { get; set; }

	public MaterialIconTextExt(MaterialIconKind kind, string text)
	{
		_kind = kind;
		_text = text;
	}

	public MaterialIconText ProvideValue()
	{
		MaterialIconText control = new()
		{
			Kind = _kind,
			Text = _text
		};
		if (Spacing != null)
			control.Spacing = Spacing.Value;
		return control;
	}

	private readonly MaterialIconKind _kind;
	private readonly string _text;
}