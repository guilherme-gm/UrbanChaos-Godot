using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeGenerators.Deserializer.Templates;
using Deserializer.Templates;
using Microsoft.CodeAnalysis;
using Stubble.Core.Builders;

namespace CodeGenerators.Deserializer;

[Generator]
public class DeserializeGenerator : ISourceGenerator
{
	private readonly Stubble.Core.StubbleVisitorRenderer Stubble = new StubbleBuilder().Build();

	public void Initialize(GeneratorInitializationContext context) {
		context.AddInitialSourcePart(DeserializeGeneratorAttribute.Template, null);
		context.AddInitialSourcePart(NestedAttribute.Template, null);
		context.FinishInitialSource("DeserializerAttributes.g.cs");

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

	private string FormatLines(string code) {
		return String.Join("\n",
			code.Split('\n')
				.Select(line => $"\t\t{line}")
		);
	}

	private string GenerateDeserializeCode(INamedTypeSymbol c) {
		var fields = new List<FieldDescriptor>();

		foreach (var member in c.GetMembers()) {
			if (member is IPropertySymbol propertySymbol)
				fields.Add(FieldDescriptor.From(propertySymbol));
			if (member is IFieldSymbol fieldSymbol && fieldSymbol.AssociatedSymbol == null)
				fields.Add(FieldDescriptor.From(fieldSymbol));
		}

		StringBuilder sb = new StringBuilder();
		foreach (var prop in fields) {
			var reader = new BasicReader(prop);
			var readerCode = this.Stubble.Render(reader.GetTemplate(), reader);
			sb.AppendLine(this.FormatLines(readerCode));
		}

		return sb.ToString();
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
		templateParams.Namespace = classNamespace != "" ? $"namespace {classNamespace}" : "";
		templateParams.Code = this.GenerateDeserializeCode(c);

		return templateParams;
	}
}
