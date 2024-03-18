using System.IO;

namespace AssetTools.UCFileStructures.Tma;

[Deserializer.DeserializeGenerator]
public partial class TextureXYSection
{
	/** Must always be 200 */
	public ushort XCount;

	/** Must always be 8 for SaveType < 5 / 5 for SaveType >= 5 */
	public ushort YCount;

	[Deserializer.DeserializeFn(FnName = nameof(DeserializeTextureXY))]
	public TextureXY[][] TextureXYs;

	public static TextureXY[][] DeserializeTextureXY(TextureXYSection section, BinaryReader br) {
		// @TODO: VariableSizedArray doesn't support multiple dimensions
		var values = new TextureXY[section.XCount][];
		for (int i = 0; i < section.XCount; i++) {
			values[i] = new TextureXY[section.YCount];
			for (int j = 0; j < section.YCount; j++) {
				values[i][j] = TextureXY.Deserialize(br);
			}
		}

		return values;
	}
}
