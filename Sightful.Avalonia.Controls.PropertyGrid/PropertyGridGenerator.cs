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

	private static void GenerateCode(SourceProductionContext context, (ImmutableArray<ViewModelData> Left, string? Right) data)
	{
		var csSource = """
		             using Avalonia.Controls;

		             namespace Generated;

		             public partial class PropertyGrid : UserControl
		             {
		             	public PropertyGrid()
		             	{
		             		InitializeComponent();
		             	}
		             }
		             """;
		IndentStringBuilder xamlBuilder = new();
		GenerateXaml(xamlBuilder, data.Left);
		if (data.Right == null)
			return;
#pragma warning disable RS1035
		Directory.CreateDirectory(Path.Combine(data.Right, "Generated"));
		File.WriteAllText(Path.Combine(data.Right, "Generated", "PropertyGrid.axaml"), xamlBuilder.ToString());
		File.WriteAllText(Path.Combine(data.Right, "Generated", "PropertyGrid.axaml.cs"), csSource);
#pragma warning restore RS1035
	}

	private static void GenerateXaml(IndentStringBuilder builder, ImmutableArray<ViewModelData> data)
	{
		var namespaces = data.Select((d, index) => (d, $"viewModels{index}")).ToImmutableArray();
		builder
			.AppendLine("<UserControl xmlns=\"https://github.com/avaloniaui\"")
			.AppendLine("             xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"")
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
		foreach (var propertySymbol in data.Properties.Where(ShouldGenerate))
			GenerateProperty(builder, propertySymbol);
	}

	private static bool ShouldGenerate(IPropertySymbol propertySymbol)
	{
		return propertySymbol.GetMethod != null && propertySymbol.SetMethod != null &&
			GetAttributeValue<bool?>(propertySymbol, "BrowsableAttribute") != false;
	}

	private static void GenerateProperty(IndentStringBuilder builder, IPropertySymbol propertySymbol)
	{
		if (propertySymbol.Type.Name == "Boolean")
		{
			var displayName = GetAttributeValue<string>(propertySymbol, "DisplayNameAttribute") ?? FormatForDisplay(propertySymbol.Name);
			var description = GetAttributeValue<string>(propertySymbol, "DescriptionAttribute") ?? string.Empty;
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

	private static T? GetAttributeValue<T>(IPropertySymbol propertySymbol, string attributeName)
	{
		var attributes = propertySymbol.GetAttributes();
		var attribute = attributes.FirstOrDefault(attribute => attribute.AttributeClass?.Name == attributeName);
		return GetAttributeValue<T>(attribute);
	}

	private static T? GetAttributeValue<T>(AttributeData? displayNameAttribute)
	{
		return (T?)displayNameAttribute?.ConstructorArguments.FirstOrDefault().Value;
	}

	// Convert "SomeString" to "Some string"
	private static string FormatForDisplay(string input)
	{
		input = SplitCamelCase(input);
		input = input.Substring(0, 1) + input.Substring(1).ToLower();
		return input;
	}

	private static string SplitCamelCase(string input)
	{
		return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
	}
}