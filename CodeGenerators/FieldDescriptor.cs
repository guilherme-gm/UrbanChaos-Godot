using System;
using System.Linq;
using Microsoft.CodeAnalysis;

public class FieldDescriptor
{
	public string Name { get; set; }

	public string FieldType { get; set; }

	public string FieldTypeFullname { get; set; }

	public ISymbol TypeSymbol { get; set; }

	public ISymbol FieldSymbol { get; set; }

	public bool IsNested;

	public bool IsArray;

	public int Rank;

	public static FieldDescriptor From(string name, INamedTypeSymbol namedSymbol) {
		return new FieldDescriptor() {
			Name = name,
			TypeSymbol = namedSymbol,
			FieldType = namedSymbol.Name,
			FieldTypeFullname = namedSymbol.ToDisplayString(),
			IsArray = false,
			Rank = 0,
		};
	}

	private static FieldDescriptor From(string name, IArrayTypeSymbol arraySymbol) {
		int rank = arraySymbol.Rank;

		var nextRank = arraySymbol;
		while (nextRank.ElementType is IArrayTypeSymbol arrayTypeSymbol) {
			rank += arrayTypeSymbol.Rank;
			nextRank = arrayTypeSymbol;
		}

		var type = nextRank.ElementType.Name;
		var typeFullname = nextRank.ElementType.ToDisplayString();

		return new FieldDescriptor() {
			Name = name,
			TypeSymbol = arraySymbol,
			FieldType = type,
			FieldTypeFullname = typeFullname,
			IsArray = true,
			Rank = rank,
		};
	}

	private static FieldDescriptor From(string name, ITypeSymbol typeSymbol) {
		if (typeSymbol is INamedTypeSymbol namedSymbol)
			return From(name, namedSymbol);

		if (typeSymbol is IArrayTypeSymbol arraySymbol)
			return From(name, arraySymbol);

		throw new Exception($"Type Symbol of Kind \"{typeSymbol.TypeKind}\" is not supported.");
	}

	public static FieldDescriptor From(IPropertySymbol property) {
		var val = From(property.Name, property.Type);
		val.IsNested = property.GetAttributes().Any((attr) => attr.AttributeClass.ToDisplayString() == "Deserializer.NestedAttribute");
		val.FieldSymbol = property;

		return val;
	}

	public static FieldDescriptor From(IFieldSymbol field) {
		var val = From(field.Name, field.Type);
		val.IsNested = field.GetAttributes().Any((attr) => attr.AttributeClass.ToDisplayString() == "Deserializer.NestedAttribute");
		val.FieldSymbol = field;

		return val;
	}
}
