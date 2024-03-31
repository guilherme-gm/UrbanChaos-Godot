using Deserializer.Templates;

namespace CodeGenerators.Deserializer.DataReaders;

public class EnumValReader : IReader
{
	public string FieldType { get; set; }
	public string FieldName { get; set; }

	public string ReadStatement { get; set; }

	public EnumValReader(FieldDescriptor descriptor) {
		this.FieldName = descriptor.Name;
		this.FieldType = descriptor.FieldTypeFullname;

		var attribute = EnumValAttribute.FromSymbol(descriptor.FieldSymbol);
		this.ReadStatement = attribute.ReadStatement;
	}

	public string GetTemplate() {
		return @"value.{{FieldName}} = ({{FieldType}}){{ReadStatement}};";
	}
}
