using Deserializer.Templates;

namespace CodeGenerators.Deserializer.Templates;

public class FixedLenStringReader : IReader
{
	public string FieldName { get; set; }

	public int StringLength { get; set; }

	public FixedLenStringReader(FieldDescriptor descriptor) {
		var attribute = FixedLenStringAttribute.FromSymbol(descriptor.FieldSymbol);
		this.StringLength = attribute.Length;

		this.FieldName = descriptor.Name;
	}

	public string GetTemplate() {
		return @"
value.{{FieldName}} = new string(br.ReadChars({{StringLength}}));
if (value.{{FieldName}}.Contains('\0')) {
	value.{{FieldName}} = value.{{FieldName}}.Substring(0, value.{{FieldName}}.IndexOf('\0'));
}
";
	}
}
