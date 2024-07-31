using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sightful.Avalonia.Controls.PropertyGrid;

[Generator]
public sealed class PropertyGridGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var classesWithAttributes = context.SyntaxProvider.ForAttributeWithMetadataName(
				"System.ComponentModel.BrowsableAttribute",
				Predicate,
				Transform)
			.Where(classSyntax => classSyntax != null)
			.Select((syntax, _) => syntax!).Collect();
		
		var projectDirectory = context.AnalyzerConfigOptionsProvider
			.Select(static (provider, _) => { 
				provider.GlobalOptions.TryGetValue("build_property.projectdir", out var projectDirectory);
				return projectDirectory;
			});
		
		context.RegisterSourceOutput(classesWithAttributes.Combine(projectDirectory), GenerateCode);
	}

	private static bool Predicate(SyntaxNode node, CancellationToken cancellationToken)
	{
		return true;
	}

	private static ViewModelData? Transform(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
	{
		if (context.TargetNode is not ClassDeclarationSyntax classSyntax)
			return null;
		var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(context.TargetNode);
		if (declaredSymbol is not INamedTypeSymbol namedTypeSymbol)
			return null;
		var containingNamespace = declaredSymbol.ContainingNamespace.ToDisplayString();
		var members = namedTypeSymbol.GetMembers().OfType<IPropertySymbol>().ToImmutableArray();
		return new ViewModelData(
			classSyntax,
			containingNamespace,
			members);
	}

	private static void GenerateCode(SourceProductionContext context, (ImmutableArray<ViewModelData> Left, string Right) data)
	{
		context.AddSource("PropertyGrid.axaml.cs",
			"""
			using Avalonia.Controls;

			namespace Generated;

			public partial class PropertyGrid : UserControl
			{
				public PropertyGrid()
				{
					InitializeComponent();
				}
			}
			"""
		);
		IndentStringBuilder xamlBuilder = new();
		GenerateXaml(xamlBuilder, data.Left);
#pragma warning disable RS1035
		File.WriteAllText(Path.Combine(data.Right, "PropertyGrid.axaml"), xamlBuilder.ToString());
#pragma warning restore RS1035
	}

	private static void GenerateXaml(IndentStringBuilder builder, ImmutableArray<ViewModelData> data)
	{
		var namespaces = data.Select((d, index) => (d, $"viewModels{index}")).ToImmutableArray();
		builder
			.AppendLine("<UserControl xmlns=\"https://github.com/avaloniaui\"")
			.AppendLine("             xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"")
			.AppendLine("             xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"")
			.AppendLine("             xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"")
			.AppendLine("             xmlns:propertyGrid=\"clr-namespace:Sightful.Avalonia.Controls.PropertyGrid;assembly=Sightful.Avalonia.Controls.PropertyGrid\"");
		GenerateNamespaces(builder, namespaces);
		builder.AppendLine("             x:Class=\"Generated.PropertyGrid\">");
		builder.IndentLevel++;
		builder.AppendIndent().AppendLine("<UserControl.DataTemplates>");
		builder.IndentLevel++;
		GenerateDataTemplates(builder, namespaces);
		builder.IndentLevel--;
		builder.AppendIndent().AppendLine("</UserControl.DataTemplates>");
		builder.IndentLevel--;
		builder.AppendLine("</UserControl>");
	}

	private static void GenerateNamespaces(
		IndentStringBuilder builder,
		IEnumerable<(ViewModelData Data, string XamlNamespace)> viewModelsData)
	{
		foreach (var (data, xamlNamespace) in viewModelsData) 
			builder
				.Append("             xmlns:")
				.Append(xamlNamespace)
				.Append("=\"clr-namespace:")
				.Append(data.ContainingNamespace)
				.AppendLine("\"");
	}

	private static void GenerateDataTemplates(
		IndentStringBuilder builder,
		IEnumerable<(ViewModelData Data, string XamlNamespace)> classes)
	{
		foreach (var (data, xamlNamespace) in classes)
		{
			builder
				.AppendIndent()
				.Append("<DataTemplate DataType=\"")
				.Append(xamlNamespace)
				.Append(':')
				.Append(data.ClassSyntax.Identifier.Text)
				.AppendLine("\">");
			builder.IndentLevel++;
			builder.AppendIndent().AppendLine("<StackPanel Spacing=\"16\">");
			builder.IndentLevel++;
			GenerateProperties(builder, data);
			builder.IndentLevel--;
			builder.AppendIndent().AppendLine("</StackPanel>");
			builder.IndentLevel--;
			builder.AppendIndent().AppendLine("</DataTemplate>");
		}
	}

	private static void GenerateProperties(IndentStringBuilder builder, ViewModelData data)
	{
		foreach (var propertySymbol in data.Properties)
			GenerateProperty(builder, propertySymbol);
	}

	private static void GenerateProperty(IndentStringBuilder builder, IPropertySymbol propertySymbol)
	{
		if (propertySymbol.Type.Name == "Boolean")
		{
			var displayName = GetAttributeStringValue(propertySymbol, "DisplayNameAttribute") ?? propertySymbol.Name;
			var description = GetAttributeStringValue(propertySymbol, "DescriptionAttribute") ?? string.Empty;
			builder.AppendIndent().Append("<ToggleSwitch IsChecked=\"{Binding ").Append(propertySymbol.Name).AppendLine("}\">");
			builder.IndentLevel++;
			builder.AppendIndent().AppendLine("<ToggleSwitch.Content>");
			builder.IndentLevel++;
			builder.AppendIndent().Append("<propertyGrid:PropertyHeader PropertyName=\"").Append(displayName).Append("\" PropertyDescription=\"").Append(description).AppendLine("\"/>");
			builder.IndentLevel--;
			builder.AppendIndent().AppendLine("</ToggleSwitch.Content>");
			builder.IndentLevel--;
			builder.AppendIndent().AppendLine("</ToggleSwitch>");
		}
	}

	private static string? GetAttributeStringValue(IPropertySymbol propertySymbol, string attributeName)
	{
		var attributes = propertySymbol.GetAttributes();
		var attribute = attributes.FirstOrDefault(attribute => attribute.AttributeClass?.Name == attributeName);
		return GetAttributeStringValue(attribute);
	}

	private static string? GetAttributeStringValue(AttributeData? displayNameAttribute)
	{
		return (string?)displayNameAttribute?.ConstructorArguments.FirstOrDefault().Value;
	}
}