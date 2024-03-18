using CodeGenerators;
using Microsoft.CodeAnalysis;
using System;

namespace Deserializer.Templates;

public class ConditionAttribute
{
	public static readonly string Name = "Deserializer.ConditionAttribute";

	public static readonly string Template = /* csharp */@"
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed class ConditionAttribute : Attribute
{
	public string When { get; set; }

	public ConditionAttribute() {
	}
}
";

	public string When { get; set; }

	public static ConditionAttribute FromSymbol(ISymbol symbol) {
		var attributeData = AttributeUtils.GetAttribute(symbol, Name);
		if (attributeData == null) {
			throw new Exception($"Could not find attribute {Name}");
		}

		var conditionString = AttributeUtils.GetAttributeField<string>(attributeData, nameof(When));

		return new ConditionAttribute() {
			When = conditionString,
		};
	}
}
