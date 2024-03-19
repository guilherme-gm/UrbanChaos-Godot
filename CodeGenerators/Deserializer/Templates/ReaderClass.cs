namespace Deserializer.Templates;

public static class ReaderClass
{
	public static readonly string Name = "Deserializer.ReaderClass";

	public static readonly string Template = /* csharp */@"
using System;
using System.IO;

{{Namespace}}

partial class {{ClassName}}
{
	partial void PostDeserialize();

	public static {{ClassName}} Deserialize(BinaryReader br)
	{
		var value = new {{ClassName}}();

{{Code}}

		value.PostDeserialize();

		return value;
	}
}
";

	public class ReaderClassTemplateParams
	{
		public string Namespace { get; set; }

		public string ClassName { get; set; }

		public string Code { get; set; }
	}
}
