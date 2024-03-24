using Microsoft.CodeAnalysis;

namespace CodeGenerators.Deserializer.DataReaders;

public class ReaderClass : IReader
{
	public string Namespace { get; set; } = "";

	public string ClassName { get; set; } = "";

	public string Code { get; set; } = "";
	public ReaderClass(INamedTypeSymbol clsSymbol) {
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

	public static {{ClassName}} Deserialize(BinaryReader br)
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
