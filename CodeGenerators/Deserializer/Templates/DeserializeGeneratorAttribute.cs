using CodeGenerators;
using Microsoft.CodeAnalysis;
using System;

namespace Deserializer.Templates;

public class DeserializeGeneratorAttribute
{
	public static readonly string Name = "Deserializer.DeserializeGeneratorAttribute";

	public static readonly string Template = /* csharp */@"
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class DeserializeGeneratorAttribute : Attribute
{
	public string[] AdditionalParams { get; set; }

	public DeserializeGeneratorAttribute() {
	}
}
";

	public string[] AdditionalParams { get; set; }

	public static DeserializeGeneratorAttribute FromSymbol(ISymbol symbol) {
		var attributeData = AttributeUtils.GetAttribute(symbol, Name);
		if (attributeData == null) {
			throw new Exception($"Could not find attribute {Name}");
		}

		var attribute = new DeserializeGeneratorAttribute();
		if (AttributeUtils.AttributeHasField(attributeData, nameof(AdditionalParams))) {
			var additionalParams = AttributeUtils.GetAttributeArrayField<string>(attributeData, nameof(AdditionalParams));
			attribute.AdditionalParams = additionalParams;
		}

		return attribute;
	}
}
