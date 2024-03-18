namespace Deserializer.Templates;

public static class DeserializeGeneratorAttribute
{
	public static readonly string Name = "Deserializer.DeserializeGeneratorAttribute";

	public static readonly string Template = /* csharp */@"

using System;

namespace Deserializer;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class DeserializeGeneratorAttribute : Attribute
{
	public DeserializeGeneratorAttribute() {
	}
}

";
}
