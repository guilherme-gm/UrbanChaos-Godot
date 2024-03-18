using System;
using CodeGenerators;
using Microsoft.CodeAnalysis;

namespace Deserializer.Templates;

public class FixedArrayAttribute
{
	public static readonly string Name = "Deserializer.FixedArrayAttribute";

	public static readonly string Template = /* csharp */@"
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed class FixedArrayAttribute : Attribute
{
	public int[] Dimensions { get; set; }

	public FixedArrayAttribute() {

	}
}
";

	public int[] Dimensions { get; set; }

	public static FixedArrayAttribute FromSymbol(ISymbol symbol) {
		var attributeData = AttributeUtils.GetAttribute(symbol, Name);
		if (attributeData == null) {
			throw new Exception($"Could not find attribute {Name}");
		}

		var dimensions = AttributeUtils.GetAttributeArrayField<int>(attributeData, nameof(Dimensions));

		return new FixedArrayAttribute() {
			Dimensions = dimensions,
		};
	}
}
