using Deserializer.Templates;

namespace CodeGenerators.Deserializer.DataReaders;

public class DeserializeFnReader : IReader
{
	public string ClassName { get; set; }
	public string FieldName { get; set; }

	public string FunctionName { get; set; }

	public DeserializeFnReader(string className, FieldDescriptor descriptor) {
		this.ClassName = className;
		this.FieldName = descriptor.Name;

		var attribute = DeserializeFnAttribute.FromSymbol(descriptor.FieldSymbol);
		this.FunctionName = attribute.FnName;
	}

	public string GetTemplate() {
		return @"value.{{FieldName}} = {{ClassName}.{{FunctionName}}(value, br);";
	}
}
