using Microsoft.CodeAnalysis;
using System.Linq;

namespace CodeGenerators;

public static class AttributeUtils
{
	public static bool HasAttribute(ISymbol symbol, string name) {
		return symbol.GetAttributes()
			.Any((attr) => attr.AttributeClass.ToDisplayString() == name);
	}
}
