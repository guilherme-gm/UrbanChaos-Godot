using Deserializer.Templates;
using Microsoft.CodeAnalysis;
using System;

namespace CodeGenerators.Deserializer.DataReaders;

public class ReaderClass : IReader
{
	public string Namespace { get; set; } = "";

	public string ClassName { get; set; } = "";

	public string Code { get; set; } = "";

	public string AdditionalParams { get; set; } = "";

	public ReaderClass(INamedTypeSymbol clsSymbol) {
		var attribute = DeserializeGeneratorAttribute.FromSymbol(clsSymbol);
		if (attribute.AdditionalParams?.Length > 0) {
			this.AdditionalParams = ", ";
			this.AdditionalParams += String.Join(", ", attribute.AdditionalParams);
		}
	}

	public string GetTemplate() {
		return /* csharp */@"
using System;
using System.IO;

{{#Namespace}}
namespace {{Namespace}};
{{/Namespace}}

partial class {{ClassName}}
{
	partial void PostDeserialize();

	public static {{ClassName}} Deserialize(BinaryReader br{{AdditionalParams}})
	{
		var value = new {{ClassName}}();

{{Code}}

		value.PostDeserialize();

		return value;
	}
}
";
	}
}
