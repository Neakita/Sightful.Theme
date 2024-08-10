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
		if (declaredSymbol.GetAttributeValue<bool?>("BrowsableAttribute") != true)
			return null;
		var containingNamespace = declaredSymbol.ContainingNamespace.ToDisplayString();
		var members = namedTypeSymbol.GetMembers();
		var properties = members.OfType<IPropertySymbol>().ToImmutableArray();
		var fields = members.OfType<IFieldSymbol>().ToImmutableArray();
		return new ViewModelData(
			classSyntax,
			containingNamespace,
			properties,
			fields);
	}

	private static void GenerateCode(SourceProductionContext context, (ImmutableArray<ViewModelData> Left, string? Right) data)
	{
		const string csSource = """
		                        using Avalonia.Controls;
		                        
		                        // ReSharper disable once CheckNamespace
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
			.AppendLine("             xmlns:propertyGrid=\"clr-namespace:Sightful.Avalonia.Controls.PropertyGrid;assembly=Sightful.Avalonia.Controls.PropertyGrid\"")
			.AppendLine("             xmlns:componentModel=\"clr-namespace:System.ComponentModel;assembly=System.ObjectModel\"");
		GenerateNamespaces(builder, namespaces);
		builder.AppendLine("             x:Class=\"Generated.PropertyGrid\"");
		builder.AppendLine("             x:DataType=\"componentModel:INotifyPropertyChanged\">");
		builder.IndentLevel++;
		builder.AppendIndent().AppendLine("<ContentControl Content=\"{Binding}\">");
		builder.IndentLevel++;
		builder.AppendIndent().AppendLine("<ContentControl.DataTemplates>");
		builder.IndentLevel++;
		GenerateDataTemplates(builder, namespaces);
		builder.IndentLevel--;
		builder.AppendIndent().AppendLine("</ContentControl.DataTemplates>");
		builder.IndentLevel--;
		builder.AppendIndent().AppendLine("</ContentControl>");
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
		var properties = data.Properties.Where(ShouldGenerate).Select(PropertyData.Create);
		var fieldsAsProperty = data.Fields.Where(ShouldGenerate).Select(PropertyData.Create);
		foreach (var property in properties)
			GenerateProperty(builder, property);
		foreach (var property in fieldsAsProperty)
			GenerateProperty(builder, property);
	}

	private static bool ShouldGenerate(IPropertySymbol symbol)
	{
		return symbol.GetMethod != null && symbol.SetMethod != null &&
		       symbol.GetAttributeValue<bool?>("BrowsableAttribute") != false;
	}

	private static bool ShouldGenerate(IFieldSymbol symbol)
	{
		return symbol.GetAttributes().Any(attribute => attribute.AttributeClass?.Name == "ObservablePropertyAttribute");
	}

	private static void GenerateProperty(IndentStringBuilder builder, PropertyData data)
	{
		if (data.Type == "Boolean")
		{
			builder.AppendIndent().Append("<ToggleSwitch HorizontalAlignment=\"Stretch\" IsChecked=\"{Binding ").Append(data.Name).AppendLine("}\">");
			builder.IndentLevel++;
			builder.AppendIndent().AppendLine("<ToggleSwitch.Content>");
			builder.IndentLevel++;
			builder.AppendIndent().Append("<propertyGrid:PropertyHeader PropertyName=\"").Append(data.DisplayName).Append("\" PropertyDescription=\"").Append(data.Description).AppendLine("\"/>");
			builder.IndentLevel--;
			builder.AppendIndent().AppendLine("</ToggleSwitch.Content>");
			builder.IndentLevel--;
			builder.AppendIndent().AppendLine("</ToggleSwitch>");
		}
	}
}