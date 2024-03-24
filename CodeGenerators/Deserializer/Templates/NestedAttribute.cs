using CodeGenerators;
using Microsoft.CodeAnalysis;
using System;

namespace Deserializer.Templates;

public class NestedAttribute
{
	public static readonly string Name = "Deserializer.NestedAttribute";

	public static readonly string Template = /* csharp */@"
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed class NestedAttribute : Attribute
{
	public string[] AdditionalParams { get; set; }

	public NestedAttribute() {

	}
}
";

	public string[] AdditionalParams { get; set; }

	public static NestedAttribute FromSymbol(ISymbol symbol) {
		var attributeData = AttributeUtils.GetAttribute(symbol, Name);
		if (attributeData == null) {
			throw new Exception($"Could not find attribute {Name}");
		}

		var attribute = new NestedAttribute();
		if (AttributeUtils.AttributeHasField(attributeData, nameof(AdditionalParams))) {
			var additionalParams = AttributeUtils.GetAttributeArrayField<string>(attributeData, nameof(AdditionalParams));
			attribute.AdditionalParams = additionalParams;
		}

		return attribute;
	}
}
