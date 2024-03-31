using Deserializer.Templates;
using System;

namespace CodeGenerators.Deserializer.DataReaders;

public class BasicReader : IReader
{
	public string FieldName { get; set; }

	public string ReadingCall { get; set; }

	public BasicReader(FieldDescriptor descriptor) {
		this.FieldName = descriptor.Name;

		if (AttributeUtils.HasAttribute(descriptor.FieldSymbol, NestedAttribute.Name)) {
			NestedAttribute nestedAttribute = NestedAttribute.FromSymbol(descriptor.FieldSymbol);
			string additionalParams = "";
			if (nestedAttribute.AdditionalParams?.Length > 0) {
				additionalParams = $", {String.Join(", ", nestedAttribute.AdditionalParams)}";
			}

			this.ReadingCall = $"{descriptor.FieldTypeFullname}.Deserialize(br{additionalParams})";
		} else if (AttributeUtils.HasAttribute(descriptor.FieldSymbol, CastValAttribute.Name)) {
			var reader = new CastValReader(descriptor);
			this.ReadingCall = reader.GetCode();
		} else {
			this.ReadingCall = $"br.Read{descriptor.FieldType}()";
		}
	}

	public string GetTemplate() {
		return @"value.{{FieldName}} = {{ReadingCall}};";
	}
}
