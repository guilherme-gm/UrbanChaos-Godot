using System.IO;

namespace AssetTools.UCFileStructures.Tma;

[Deserializer.DeserializeGenerator]
public partial class TextureInfoSection
{
	[Deserializer.DeserializeFn(FnName = nameof(DeserializeCount))]
	public ushort Count;

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(Count))]
	[Deserializer.Nested]
	public TextureInfo[] TextureInfos;

#pragma warning disable IDE0060 // Remove unused parameter
	public static ushort DeserializeCount(TextureInfoSection section, BinaryReader br) {
		return (ushort)(br.ReadUInt16() * 8 * 8);
	}
#pragma warning restore IDE0060 // Remove unused parameter
}
