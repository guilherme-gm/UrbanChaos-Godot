namespace AssetTools.UCFileStructures.MultiPrim;

[Deserializer.DeserializeGenerator]
public partial class GameKeyFrameElement
{
	[Deserializer.Nested]
	public CMatrix33 CMatrix { get; set; }
	public short OffsetX { get; set; }
	public short OffsetY { get; set; }
	public short OffsetZ { get; set; }
	public ushort Pad { get; set; }
}
