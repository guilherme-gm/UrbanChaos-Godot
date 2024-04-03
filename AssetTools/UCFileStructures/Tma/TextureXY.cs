using AssetTools.UCFileStructures.Maps.SuperMap;

namespace AssetTools.UCFileStructures.Tma;

// TXTY
[Deserializer.DeserializeGenerator]
public partial class TextureXY
{
	public byte Page { get; set; }

	public byte Tx { get; set; }

	public byte Ty { get; set; }

	[Deserializer.CastVal(ReadStatement = "br.ReadByte()")]
	public TextureFlip Flip { get; set; }
};
