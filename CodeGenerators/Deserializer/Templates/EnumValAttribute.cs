using CodeGenerators;
using Microsoft.CodeAnalysis;
using System;

namespace Deserializer.Templates;

public class EnumValAttribute
{
	public static readonly string Name = "Deserializer.EnumValAttribute";

	public static readonly string Template = /* csharp */@"
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed class EnumValAttribute : Attribute
{
	public string ReadStatement { get; set; }

	public EnumValAttribute() {
	}
}
";

	public string ReadStatement { get; set; }

	public static EnumValAttribute FromSymbol(ISymbol symbol) {
		var attributeData = AttributeUtils.GetAttribute(symbol, Name);
		if (attributeData == null) {
			throw new Exception($"Could not find attribute {Name}");
		}

		var readCall = AttributeUtils.GetAttributeField<string>(attributeData, nameof(ReadStatement));

		return new EnumValAttribute() {
			ReadStatement = readCall,
		};
	}
}
