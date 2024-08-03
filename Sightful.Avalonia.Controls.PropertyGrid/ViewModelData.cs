using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sightful.Avalonia.Controls.PropertyGrid;

internal sealed class ViewModelData
{
	public ClassDeclarationSyntax ClassSyntax { get; }
	public string ContainingNamespace { get; }
	public ImmutableArray<IPropertySymbol> Properties { get; }
	public ImmutableArray<IFieldSymbol> Fields { get; }

	public ViewModelData(
		ClassDeclarationSyntax classSyntax,
		string containingNamespace,
		ImmutableArray<IPropertySymbol> properties,
		ImmutableArray<IFieldSymbol> fields)
	{
		ClassSyntax = classSyntax;
		ContainingNamespace = containingNamespace;
		Properties = properties;
		Fields = fields;
	}
}