namespace Deserializer.Templates;

public class SkipAttribute
{
	public static readonly string Name = "Deserializer.SkipAttribute";

	public static readonly string Template = /* csharp */@"
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed class SkipAttribute : Attribute
{
	public SkipAttribute() {

	}
}
";
}
