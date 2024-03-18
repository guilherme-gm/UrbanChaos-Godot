using System.IO;

namespace AssetTools.UCFileStructures.Tma;

[Deserializer.DeserializeGenerator]
public partial class TextureStyleNameSection
{
	/** Must always be 200 */
	public ushort StringCount;

	/** Must always be 21 */
	public ushort StringLength;

	[Deserializer.DeserializeFn(FnName = nameof(DeserializeNames))]
	public string[] Names;

	public static string[] DeserializeNames(TextureStyleNameSection section, BinaryReader br) {
		// @TODO: VariableSizedArray doesn't support strings
		var values = new string[section.StringCount];
		for (int i = 0; i < section.StringCount; i++) {
			var name = new string(br.ReadChars(section.StringLength));
			if (name.Contains('\0')) {
				name = name[..name.IndexOf('\0')];
			}

			values[i] = name;
		}

		return values;
	}
}
