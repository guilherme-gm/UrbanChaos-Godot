using System;
using CodeGenerators;
using Microsoft.CodeAnalysis;

namespace Deserializer.Templates;

public class FixedLenStringAttribute
{
	public static readonly string Name = "Deserializer.FixedLenStringAttribute";

	public static readonly string Template = /* csharp */@"
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed class FixedLenStringAttribute : Attribute
{
	public int Length { get; set; }

	public FixedLenStringAttribute() {

	}
}
";

	public int Length { get; set; }

	public static FixedLenStringAttribute FromSymbol(ISymbol symbol) {
		var attributeData = AttributeUtils.GetAttribute(symbol, Name);
		if (attributeData == null) {
			throw new Exception($"Could not find attribute {Name}");
		}

		var length = AttributeUtils.GetAttributeField<int>(attributeData, nameof(Length));

		return new FixedLenStringAttribute() {
			Length = length,
		};
	}
}
