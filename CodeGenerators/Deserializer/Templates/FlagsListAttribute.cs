using CodeGenerators;
using Microsoft.CodeAnalysis;
using System;

namespace Deserializer.Templates;

public class FlagsListAttribute
{
	public static readonly string Name = "Deserializer.FlagsListAttribute";

	public static readonly string Template = /* csharp */@"
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed class FlagsListAttribute : Attribute
{
	public string ReadStatement { get; set; }

	public FlagsListAttribute() {
	}
}
";

	public string ReadStatement { get; set; }

	public static FlagsListAttribute FromSymbol(ISymbol symbol) {
		var attributeData = AttributeUtils.GetAttribute(symbol, Name);
		if (attributeData == null) {
			throw new Exception($"Could not find attribute {Name}");
		}

		var readCall = AttributeUtils.GetAttributeField<string>(attributeData, nameof(ReadStatement));

		return new FlagsListAttribute() {
			ReadStatement = readCall,
		};
	}
}
