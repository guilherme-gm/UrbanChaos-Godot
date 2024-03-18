using Deserializer.Templates;

namespace CodeGenerators.Deserializer.DataReaders;

public class VariableSizedArrayReader : IReader
{
	public string FieldName { get; set; }

	public string FieldType { get; set; }

	public string ReadCall { get; set; }

	public string SizeFieldName { get; set; }

	public VariableSizedArrayReader(FieldDescriptor descriptor) {
		this.FieldName = descriptor.Name;
		this.FieldType = descriptor.FieldType;

		var attribute = VariableSizedArrayAttribute.FromSymbol(descriptor.FieldSymbol);
		this.SizeFieldName = attribute.SizePropertyName;

		if (AttributeUtils.HasAttribute(descriptor.FieldSymbol, NestedAttribute.Name)) {
			this.ReadCall = $"{descriptor.FieldType}.Deserialize(br)";
		} else {
			this.ReadCall = $"br.Read{descriptor.FieldType}()";
		}
	}

	public string GetTemplate() {
		return @"
value.{{FieldName}} = new {{FieldType}}[value.{{SizeFieldName}}];
for (int i = 0; i < value.{{SizeFieldName}}; i++) {
	value.{{FieldName}}[i] = {{ReadCall}};
}
";
	}
}
