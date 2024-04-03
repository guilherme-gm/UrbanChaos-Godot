using AssetTools.Utils;
using System.IO;

namespace AssetTools.UCFileStructures.Tma;

[Deserializer.DeserializeGenerator]
public partial class TextureFlagsSection
{
	public ushort Count1;

	public ushort Count2;

	[Deserializer.DeserializeFn(FnName = nameof(DeserializeFlags))]
	public Flags<FaceFlag>[][] Flags;

	public static Flags<FaceFlag>[][] DeserializeFlags(TextureFlagsSection section, BinaryReader br) {
		// @TODO: VariableSizedArray doesn't support multiple dimensions
		var value = new Flags<FaceFlag>[section.Count1][];
		for (int i = 0; i < section.Count1; i++) {
			value[i] = new Flags<FaceFlag>[section.Count2];

			for (int j = 0; j < section.Count2; j++) {
				value[i][j] = FaceFlag.FromNumber(br.ReadByte());
			}
		}

		return value;
	}
}
