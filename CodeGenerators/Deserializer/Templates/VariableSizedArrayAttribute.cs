using System;
using CodeGenerators;
using Microsoft.CodeAnalysis;

namespace Deserializer.Templates;

public class VariableSizedArrayAttribute
{
	public static readonly string Name = "Deserializer.VariableSizedArrayAttribute";

	public static readonly string Template = /* csharp */@"
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed class VariableSizedArrayAttribute : Attribute
{
	public string SizePropertyName { get; set; }

	public VariableSizedArrayAttribute() {

	}
}
";

	public string SizePropertyName { get; set; }

	public static VariableSizedArrayAttribute FromSymbol(ISymbol symbol) {
		var attributeData = AttributeUtils.GetAttribute(symbol, Name);
		if (attributeData == null) {
			throw new Exception($"Could not find attribute {Name}");
		}

		var propertyName = AttributeUtils.GetAttributeField<string>(attributeData, nameof(SizePropertyName));

		return new VariableSizedArrayAttribute() {
			SizePropertyName = propertyName,
		};
	}
}
