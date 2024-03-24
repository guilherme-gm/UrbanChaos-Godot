using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeGenerators;

public static class AttributeUtils
{
	public static bool HasAttribute(ISymbol symbol, string name) {
		return symbol.GetAttributes()
			.Any((attr) => attr.AttributeClass.ToDisplayString() == name);
	}

	public static AttributeData GetAttribute(ISymbol symbol, string name) {
		return symbol.GetAttributes()
			.SingleOrDefault((attr) => attr.AttributeClass.ToDisplayString() == name);
	}

	public static T[] GetAttributeArrayField<T>(AttributeData attribute, string name) {
		var val = attribute.NamedArguments.Single((param) => param.Key == name).Value;
		if (val.IsNull) {
			throw new Exception($"Could not find field \"${name}\" in attribute \"{attribute.AttributeClass.Name}\"");
		}

		var values = new List<T>();
		foreach (var value in val.Values) {
			values.Add((T)value.Value);
		}

		return [.. values];
	}

	public static T GetAttributeField<T>(AttributeData attribute, string name) {
		var val = attribute.NamedArguments.Single((param) => param.Key == name).Value;

		if (val.IsNull) {
			throw new Exception($"Could not find field \"${name}\" in attribute \"{attribute.AttributeClass.Name}\"");
		}

		return (T)val.Value;
	}

	public static bool AttributeHasField(AttributeData attribute, string name) {
		var val = attribute?.NamedArguments.SingleOrDefault((param) => param.Key == name).Value;

		return !(val?.IsNull ?? true);
	}
}
