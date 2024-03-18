using Deserializer.Templates;
using System.Text;

namespace CodeGenerators.Deserializer.Templates;

public class FixedArrayReader : IReader
{
	public string FieldName { get; set; }

	public string ReadingCall { get; set; }

	public string ReadCode { get; set; }

	public int[] Dimensions { get; set; }

	public FixedArrayReader(FieldDescriptor descriptor) {
		var attribute = FixedArrayAttribute.FromSymbol(descriptor.FieldSymbol);
		this.Dimensions = attribute.Dimensions;

		this.FieldName = descriptor.Name;

		if (AttributeUtils.HasAttribute(descriptor.FieldSymbol, NestedAttribute.Name)) {
			this.ReadingCall = $"{descriptor.FieldType}.Deserialize(br)";
		} else {
			this.ReadingCall = $"br.Read{descriptor.FieldType}()";
		}

		this.ReadCode = this.GetReadCode();
	}

	private string GetReadCode(int depth = 0) {
		var indent = "".PadLeft(depth, '\t');

		StringBuilder sb = new StringBuilder();
		_ = sb.AppendLine($"{indent}[");

		if (depth == this.Dimensions.Length - 1) {
			_ = sb.Append($"{indent}\t");
			for (int i = 0; i < this.Dimensions[depth] - 1; i++) {
				_ = sb.Append($"{this.ReadingCall}, ");
			}
			_ = sb.AppendLine($"{this.ReadingCall}");
		} else {
			_ = sb.Append(this.GetReadCode(depth + 1));
		}

		if (depth > 0)
			_ = sb.AppendLine($"{indent}],");
		else
			_ = sb.Append($"{indent}]");

		return sb.ToString();
	}

	public string GetTemplate() {
		return @"value.{{FieldName}} = {{ReadCode}};";
	}
}
