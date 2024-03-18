using CodeGenerators;
using Microsoft.CodeAnalysis;
using System;

namespace Deserializer.Templates;

public class DeserializeFnAttribute
{
	public static readonly string Name = "Deserializer.DeserializeFnAttribute";

	public static readonly string Template = /* csharp */@"
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed class DeserializeFnAttribute : Attribute
{
	public string FnName { get; set; }

	public DeserializeFnAttribute() {
	}
}
";

	public string FnName { get; set; }

	public static DeserializeFnAttribute FromSymbol(ISymbol symbol) {
		var attributeData = AttributeUtils.GetAttribute(symbol, Name);
		if (attributeData == null) {
			throw new Exception($"Could not find attribute {Name}");
		}

		var propertyName = AttributeUtils.GetAttributeField<string>(attributeData, nameof(FnName));

		return new DeserializeFnAttribute() {
			FnName = propertyName,
		};
	}
}
