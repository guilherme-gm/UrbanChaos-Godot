namespace AssetTools.UCFileStructures.Maps;

/** The objects -- Originally Ob_ob */
[Deserializer.DeserializeGenerator]
public partial class MapObject
{
	public short Y { get; set; }
	public byte X { get; set; }
	public byte Z { get; set; }
	public byte Prim { get; set; }
	public byte Yaw { get; set; }
	public byte Flags { get; set; }
	public byte InsideIndex { get; set; }
}
