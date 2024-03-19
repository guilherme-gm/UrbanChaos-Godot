namespace AssetTools.UCFileStructures.Prim;

[Deserializer.DeserializeGenerator]
public partial class PrimFace4 : PrimFace, IPrimFace
{
	public byte CompressedTexturePage { get; set; }

	/*
	#define	POLY_FLAG_GOURAD		(1<<0)
	#define	POLY_FLAG_TEXTURED		(1<<1)
	#define	POLY_FLAG_MASKED		(1<<2)
	#define	POLY_FLAG_SEMI_TRANS	(1<<3)
	#define	POLY_FLAG_ALPHA			(1<<4)
	#define	POLY_FLAG_TILED			(1<<5)
	#define	POLY_FLAG_DOUBLESIDED	(1<<6)
	#define	POLY_FLAG_WALKABLE		(1<<7)
	*/
	public byte DrawFlags { get; set; }

	/**
	 * Points that forms that face. They are array positions in PrimFile.
	 * Note: In the file structure, these are numbered as StartPoint + index,
	 *       but Deserialize adjust that.
	 */
	[Deserializer.FixedArray(Dimensions = [4])]
	public ushort[] Points { get; set; } // [4]

	[Deserializer.FixedArray(Dimensions = [4, 2])]
	public byte[][] CompressedUV { get; set; } // [4][2]


	[Deserializer.FixedArray(Dimensions = [4])]
	public short[] Bright { get; set; }  // [4] Used for people. May also be 3 bytes R / G / B

	public short ThingIndex { get; set; }
	public short Col2 { get; set; }
	public ushort FaceFlags { get; set; }
	public byte Type { get; set; }      // move after bright
	public byte ID { get; set; }

	public void FixPointIds(ushort startPointId) {
		// The official structure starts counting points (usually 1, but this comes from the file)
		// so here we compensate it to array position
		for (int i = 0; i < this.Points.Length; i++) {
			this.Points[i] -= startPointId;
		}
	}
}

