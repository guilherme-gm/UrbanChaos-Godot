using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeGenerators.Deserializer.DataReaders;
using Deserializer.Templates;
using Microsoft.CodeAnalysis;
using Stubble.Core.Builders;

namespace CodeGenerators.Deserializer;

[Generator]
public class DeserializeGenerator : ISourceGenerator
{
	private readonly Stubble.Core.StubbleVisitorRenderer Stubble = new StubbleBuilder()
		.Configure(settings => {
			// Disable encoding, this is meant for HTML, and we are doing C# code here...
			_ = settings.SetEncodingFunction(val => val);
		})
		.Build();

	private readonly StringBuilder InitialSource = new StringBuilder()
		.AppendLine("using System;")
		.AppendLine("namespace Deserializer;");

	public void Initialize(GeneratorInitializationContext context) {
		this.AddInitialSourcePart(ConditionAttribute.Template, null);
		this.AddInitialSourcePart(DeserializeGeneratorAttribute.Template, null);
		this.AddInitialSourcePart(DeserializeFnAttribute.Template, null);
		this.AddInitialSourcePart(FixedArrayAttribute.Template, null);
		this.AddInitialSourcePart(FixedLenStringAttribute.Template, null);
		this.AddInitialSourcePart(NestedAttribute.Template, null);
		this.AddInitialSourcePart(SkipAttribute.Template, null);
		this.AddInitialSourcePart(VariableSizedArrayAttribute.Template, null);
		this.AddInitialSourcePart(FlagsListAttribute.Template, null);
		this.AddInitialSourcePart(CastValAttribute.Template, null);
		this.FinishInitialSource(context, "DeserializerAttributes.g.cs");

		context.RegisterForSyntaxNotifications(() => new DeserializerSyntaxReceiver());
	}

	public void Execute(GeneratorExecutionContext context) {
		if (!(context.SyntaxContextReceiver is DeserializerSyntaxReceiver receiver))
			return;

		foreach (var targetClass in receiver.TargetClasses) {
			var readerClass = this.GenerateDeserializeClass(targetClass);
			this.AddSourceFromTemplate(
				context,
				$"{readerClass.ClassName}_deserialize.g",
				readerClass.GetTemplate(),
				readerClass
			);
		}
	}

	private void AddInitialSourcePart(string template, object data) {
		var codeToAdd = this.Stubble.Render(template, data);
		_ = this.InitialSource.AppendLine(codeToAdd);
	}

	private void FinishInitialSource(GeneratorInitializationContext context, string fileName) {
		context.RegisterForPostInitialization((i) => i.AddSource(fileName, this.InitialSource.ToString()));
	}

	private void AddSourceFromTemplate(GeneratorExecutionContext context, string fileName, string template, object data) {
		var codeToAdd = this.Stubble.Render(template, data);
		context.AddSource(fileName, codeToAdd);
	}

	private string FormatLines(string code) {
		return String.Join("\n",
			code.Split('\n')
				.Select(line => $"\t\t{line}")
		);
	}

	private string GenerateDeserializeCode(string className, INamedTypeSymbol c) {
		var fields = new List<FieldDescriptor>();

		foreach (var member in c.GetMembers()) {
			if (member is IPropertySymbol propertySymbol)
				fields.Add(FieldDescriptor.From(propertySymbol));
			if (member is IFieldSymbol fieldSymbol && fieldSymbol.AssociatedSymbol == null)
				fields.Add(FieldDescriptor.From(fieldSymbol));
		}

		StringBuilder sb = new StringBuilder();
		foreach (var prop in fields) {
			if (AttributeUtils.HasAttribute(prop.FieldSymbol, SkipAttribute.Name)) {
				continue;
			}

			IReader reader;
			if (AttributeUtils.HasAttribute(prop.FieldSymbol, CastValAttribute.Name)) {
				reader = new CastValReader(prop);
			} else if (AttributeUtils.HasAttribute(prop.FieldSymbol, FlagsListAttribute.Name)) {
				reader = new FlagsListReader(prop);
			} else if (AttributeUtils.HasAttribute(prop.FieldSymbol, VariableSizedArrayAttribute.Name)) {
				reader = new VariableSizedArrayReader(prop);
			} else if (AttributeUtils.HasAttribute(prop.FieldSymbol, FixedLenStringAttribute.Name)) {
				reader = new FixedLenStringReader(prop);
			} else if (AttributeUtils.HasAttribute(prop.FieldSymbol, FixedArrayAttribute.Name)) {
				reader = new FixedArrayReader(prop);
			} else if (AttributeUtils.HasAttribute(prop.FieldSymbol, DeserializeFnAttribute.Name)) {
				reader = new DeserializeFnReader(className, prop);
			} else {
				reader = new BasicReader(prop);
			}

			var readerCode = this.Stubble.Render(reader.GetTemplate(), reader);
			if (AttributeUtils.HasAttribute(prop.FieldSymbol, ConditionAttribute.Name)) {
				var conditionReader = new ConditionReader(prop, readerCode);
				readerCode = this.Stubble.Render(conditionReader.GetTemplate(), conditionReader);
			}

			_ = sb.AppendLine(this.FormatLines(readerCode));
		}

		return sb.ToString();
	}

	private ReaderClass GenerateDeserializeClass(INamedTypeSymbol clsSymbol) {
		var readerClass = new ReaderClass(clsSymbol);

		var className = clsSymbol.ToDisplayString();
		if (!clsSymbol.ContainingNamespace.IsGlobalNamespace) {
			var classNamespace = clsSymbol.ContainingNamespace.ToDisplayString();
			className = className.Replace($"{classNamespace}.", "");
			readerClass.Namespace = classNamespace;
		}

		readerClass.ClassName = className;
		readerClass.Code = this.GenerateDeserializeCode(className, clsSymbol);

		return readerClass;
	}
}
