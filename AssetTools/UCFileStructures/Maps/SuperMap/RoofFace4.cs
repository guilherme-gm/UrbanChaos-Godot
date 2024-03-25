namespace AssetTools.UCFileStructures.Maps.SuperMap;

[Deserializer.DeserializeGenerator]
public partial class RoofFace4
{
	//	UWORD	TexturePage; //could use the texture on the floor
	public short Y { get; set; }
	[Deserializer.FixedArray(Dimensions = [3])]
	public sbyte[] DY { get; set; }
	public byte DrawFlags { get; set; }
	public byte RX { get; set; }
	public byte RZ { get; set; }
	/** //link list of walkables off floor  */
	public short Next { get; set; }
}
