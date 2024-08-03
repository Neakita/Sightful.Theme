using Microsoft.CodeAnalysis;

namespace Sightful.Avalonia.Controls.PropertyGrid;

internal sealed class PropertyData
{
	public static PropertyData Create(IPropertySymbol symbol)
	{
		var displayName = symbol.GetAttributeValue<string>("DisplayNameAttribute") ?? symbol.Name.FormatForDisplay();
		var description = symbol.GetAttributeValue<string>("DescriptionAttribute") ?? string.Empty;
		return new PropertyData(symbol.Type.Name, symbol.Name, displayName, description);
	}

	public static PropertyData Create(IFieldSymbol symbol)
	{
		var propertyName = symbol.Name.ToPropertyName();
		var displayName = symbol.GetAttributeValue<string>("DisplayNameAttribute") ?? propertyName.FormatForDisplay();
		var description = symbol.GetAttributeValue<string>("DescriptionAttribute") ?? string.Empty;
		return new PropertyData(symbol.Type.Name, propertyName, displayName, description);
	}

	public string Type { get; }
	public string Name { get; }
	public string DisplayName { get; }
	public string Description { get; }

	public PropertyData(string type, string name, string displayName, string description)
	{
		Type = type;
		Name = name;
		DisplayName = displayName;
		Description = description;
	}

	public override bool Equals(object? obj)
	{
		return ReferenceEquals(this, obj) || obj is PropertyData other && Equals(other);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			var hashCode = Type.GetHashCode();
			hashCode = (hashCode * 397) ^ Name.GetHashCode();
			hashCode = (hashCode * 397) ^ DisplayName.GetHashCode();
			hashCode = (hashCode * 397) ^ Description.GetHashCode();
			return hashCode;
		}
	}

	private bool Equals(PropertyData other)
	{
		return Type == other.Type && Name == other.Name && DisplayName == other.DisplayName && Description == other.Description;
	}
}