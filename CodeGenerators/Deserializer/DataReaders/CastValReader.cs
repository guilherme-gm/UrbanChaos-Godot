using Deserializer.Templates;

namespace CodeGenerators.Deserializer.DataReaders;

public class CastValReader : IReader
{
	public string FieldType { get; set; }
	public string FieldName { get; set; }

	public string ReadStatement { get; set; }

	public CastValReader(FieldDescriptor descriptor) {
		this.FieldName = descriptor.Name;
		this.FieldType = descriptor.FieldTypeFullname;

		var attribute = CastValAttribute.FromSymbol(descriptor.FieldSymbol);
		this.ReadStatement = attribute.ReadStatement;
	}

	public string GetTemplate() {
		return @"value.{{FieldName}} = ({{FieldType}}){{ReadStatement}};";
	}

	public string GetCode() {
		return $"({this.FieldType}){this.ReadStatement}";
	}
}
