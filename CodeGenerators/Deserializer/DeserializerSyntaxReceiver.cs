using System.Collections.Generic;
using System.Linq;
using Deserializer.Templates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenerators.Deserializer;

internal class DeserializerSyntaxReceiver : ISyntaxContextReceiver
{
	public List<INamedTypeSymbol> TargetClasses { get; } = [];

	/// <summary>
	/// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
	/// </summary>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
		if (!(context.Node is ClassDeclarationSyntax)) {
			return;
		}

		var classDeclarationSyntax = context.Node as ClassDeclarationSyntax;
		var clsSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;
		if (clsSymbol == null) {
			throw new System.Exception($"named symbol is not INamedType -- {context.Node}");
		}

		var attrs = clsSymbol.GetAttributes();
		var hasGeneratorAttr = attrs.Any((attr) => attr.AttributeClass.ToDisplayString() == DeserializeGeneratorAttribute.Name);

		if (hasGeneratorAttr) {
			this.TargetClasses.Add(clsSymbol);
		}
	}
}
