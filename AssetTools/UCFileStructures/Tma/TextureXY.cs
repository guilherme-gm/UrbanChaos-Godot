namespace AssetTools.UCFileStructures.Tma;

// TXTY
[Deserializer.DeserializeGenerator]
public partial class TextureXY
{
	public byte Page { get; set; }
	public byte Tx { get; set; }
	public byte Ty { get; set; }
	public byte Flip { get; set; }
};
