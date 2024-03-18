using Deserializer.Templates;

namespace CodeGenerators.Deserializer.Templates;

public class BasicReader : IReader
{
	public string FieldName { get; set; }

	public string ReadingCall { get; set; }

	public BasicReader(FieldDescriptor descriptor) {
		this.FieldName = descriptor.Name;

		if (AttributeUtils.HasAttribute(descriptor.FieldSymbol, NestedAttribute.Name)) {
			this.ReadingCall = $"{descriptor.FieldType}.Deserialize(br)";
		} else {
			this.ReadingCall = $"br.Read{descriptor.FieldType}()";
		}
	}

	public string GetTemplate() {
		return @"
value.{{FieldName}} = {{ReadingCall}};
";
	}
}
