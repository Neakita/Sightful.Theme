using System.Text;

namespace Sightful.Avalonia.Controls.PropertyGrid;

internal sealed class IndentStringBuilder
{
	public byte IndentLevel { get; set; }

	public IndentStringBuilder AppendLine(string value)
	{
		_stringBuilder.AppendLine(value);
		return this;
	}

	public IndentStringBuilder Append(string value)
	{
		_stringBuilder.Append(value);
		return this;
	}

	public IndentStringBuilder Append(char value)
	{
		_stringBuilder.Append(value);
		return this;
	}

	public IndentStringBuilder AppendIndent()
	{
		_stringBuilder.Append('\t', IndentLevel);
		return this;
	}

	public override string ToString()
	{
		return _stringBuilder.ToString();
	}

	private readonly StringBuilder _stringBuilder = new();
}