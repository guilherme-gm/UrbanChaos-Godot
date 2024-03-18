using Deserializer.Templates;
using System;
using System.Linq;

namespace CodeGenerators.Deserializer.DataReaders;

public class ConditionReader : IReader
{
	public string Condition { get; set; }
	public string ReaderCode { get; set; }

	private string IndentLines(string code) {
		return String.Join("\n",
			code.Split('\n')
				.Select(line => $"\t{line}")
		);
	}

	public ConditionReader(FieldDescriptor descriptor, string readerCode) {
		this.ReaderCode = this.IndentLines(readerCode);

		var attribute = ConditionAttribute.FromSymbol(descriptor.FieldSymbol);
		this.Condition = attribute.When;
	}

	public string GetTemplate() {
		return @"
if ({{Condition}}) {
{{ReaderCode}}
}
";
	}
}
