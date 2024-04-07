using AssetTools.Utils;

namespace AssetTools.UCFileStructures.Maps.SuperMap;

[Deserializer.DeserializeGenerator]
public partial class RoofFace4
{
	//	UWORD	TexturePage; //could use the texture on the floor
	public short Y { get; set; }

	[Deserializer.FixedArray(Dimensions = [3])]
	public sbyte[] DY { get; set; }

	[Deserializer.FlagsList(ReadStatement = "br.ReadByte()")]
	public Flags<RoofFace4DrawFlags> DrawFlags { get; set; }

	private byte CompressedRX { get; set; }

	[Deserializer.Skip]
	public HighResRoofCoordinate RX { get; set; }

	private byte CompressedRZ { get; set; }

	[Deserializer.Skip]
	public HighResRoofCoordinate RZ { get; set; }

	/** //link list of walkables off floor  */
	public short Next { get; set; }

	partial void PostDeserialize() {
		if (this.DY[0] != 0 || this.DY[1] != 0 || this.DY[2] != 0) {
			this.RZ = new HighResRoofCoordinate(
				new HighResCoordinate(this.CompressedRZ),
				1
			);
		} else {
			this.RZ = new HighResRoofCoordinate(
				new HighResCoordinate(this.CompressedRZ & 127),
				0
			);
		}

		this.RX = new HighResRoofCoordinate(new HighResCoordinate(this.CompressedRX), 0);
	}
}
