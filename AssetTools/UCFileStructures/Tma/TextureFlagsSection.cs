using System.IO;

namespace AssetTools.UCFileStructures.Tma;

[Deserializer.DeserializeGenerator]
public partial class TextureFlagsSection
{
	public ushort Count1;

	public ushort Count2;

	[Deserializer.DeserializeFn(FnName = nameof(DeserializeFlags))]
	public byte[][] Flags;

	public static byte[][] DeserializeFlags(TextureFlagsSection section, BinaryReader br) {
		// @TODO: VariableSizedArray doesn't support multiple dimensions
		var value = new byte[section.Count1][];
		for (int i = 0; i < section.Count1; i++) {
			value[i] = new byte[section.Count2];

			for (int j = 0; j < section.Count2; j++) {
				value[i][j] = br.ReadByte();
			}
		}

		return value;
	}
}
