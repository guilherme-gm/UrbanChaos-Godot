using AssetTools.Utils;

namespace AssetTools.UCFileStructures.Maps.SuperMap;

[Deserializer.DeserializeGenerator]
public partial class RoofFace4
{
	//	UWORD	TexturePage; //could use the texture on the floor
	public short Y { get; set; }

	[Deserializer.FixedArray(Dimensions = [3])]
	public sbyte[] DY { get; set; }

	[Deserializer.CastVal(ReadStatement = "br.ReadByte()")]
	public Flags<RoofFace4DrawFlags> DrawFlags { get; set; }

	public byte RX { get; set; }

	public byte RZ { get; set; }

	/** //link list of walkables off floor  */
	public short Next { get; set; }

	partial void PostDeserialize() {
		if (this.DY[0] != 0 || this.DY[1] != 0 || this.DY[2] != 0) {
			this.RZ |= 1 << 7;
		} else {
			this.RZ &= 127;
		}
	}
}
