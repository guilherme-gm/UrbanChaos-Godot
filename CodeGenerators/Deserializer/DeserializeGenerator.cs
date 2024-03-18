using System;
using Deserializer.Templates;
using Microsoft.CodeAnalysis;

namespace CodeGenerators.Deserializer;

[Generator]
public class DeserializeGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context) {
		context.AddSourceFromTemplate("DeserializeGeneratorAttribute.g.cs", DeserializeGeneratorAttribute.Template, null);

		context.RegisterForSyntaxNotifications(() => new DeserializerSyntaxReceiver());
	}

	public void Execute(GeneratorExecutionContext context) {
		if (!(context.SyntaxContextReceiver is DeserializerSyntaxReceiver receiver))
			return;

		foreach (var targetClass in receiver.TargetClasses) {
			var generatorParams = this.GenerateDeserializeClass(targetClass);
			context.AddSourceFromTemplate(
				$"{generatorParams.ClassName}_deserialize.g",
				ReaderClass.Template,
				generatorParams
			);
		}
	}

	private ReaderClass.ReaderClassTemplateParams GenerateDeserializeClass(INamedTypeSymbol c) {
		var templateParams = new ReaderClass.ReaderClassTemplateParams();

		var className = c.ToDisplayString();
		string classNamespace = String.Empty;
		if (!c.ContainingNamespace.IsGlobalNamespace) {
			classNamespace = c.ContainingNamespace.ToDisplayString();
			className = className.Replace($"{classNamespace}.", "");
		}

		templateParams.ClassName = className;
		templateParams.Namespace = classNamespace;

		templateParams.Code = "// @TODO:";

		return templateParams;
	}
}
