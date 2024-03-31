using CodeGenerators;
using Microsoft.CodeAnalysis;
using System;

namespace Deserializer.Templates;

public class CastValAttribute
{
	public static readonly string Name = "Deserializer.CastValAttribute";

	public static readonly string Template = /* csharp */@"
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed class CastValAttribute : Attribute
{
	public string ReadStatement { get; set; }

	public CastValAttribute() {
	}
}
";

	public string ReadStatement { get; set; }

	public static CastValAttribute FromSymbol(ISymbol symbol) {
		var attributeData = AttributeUtils.GetAttribute(symbol, Name);
		if (attributeData == null) {
			throw new Exception($"Could not find attribute {Name}");
		}

		var readCall = AttributeUtils.GetAttributeField<string>(attributeData, nameof(ReadStatement));

		return new CastValAttribute() {
			ReadStatement = readCall,
		};
	}
}
