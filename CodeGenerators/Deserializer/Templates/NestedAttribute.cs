namespace Deserializer.Templates;

public static class NestedAttribute
{
	public static readonly string Name = "Deserializer.NestedAttribute";

	public static readonly string Template = /* csharp */@"
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed class NestedAttribute : Attribute
{
	public NestedAttribute() {

	}
}
";
}
