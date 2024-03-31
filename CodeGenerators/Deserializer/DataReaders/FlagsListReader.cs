using Deserializer.Templates;
using Microsoft.CodeAnalysis;

namespace CodeGenerators.Deserializer.DataReaders;

public class FlagsListReader : IReader
{
	public string FieldType { get; set; }
	public string FieldName { get; set; }

	public string ReadStatement { get; set; }

	public FlagsListReader(FieldDescriptor descriptor) {
		this.FieldName = descriptor.Name;
		this.FieldType = (descriptor.TypeSymbol as INamedTypeSymbol).TypeArguments[0].ToDisplayString();

		var attribute = FlagsListAttribute.FromSymbol(descriptor.FieldSymbol);
		this.ReadStatement = attribute.ReadStatement;
	}

	public string GetTemplate() {
		return @"value.{{FieldName}} = {{FieldType}}.FromNumber({{ReadStatement}});";
	}
}
