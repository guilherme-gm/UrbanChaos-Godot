namespace AssetTools.UCFileStructures.Prim;

public interface IPrimFace
{
	public ushort[] Points { get; set; }

	public ushort FaceFlags { get; set; }

	public byte[][] CompressedUV { get; set; } // [3][2]

	public byte CompressedTexturePage { get; set; }
}
