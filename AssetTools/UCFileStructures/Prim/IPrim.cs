namespace AssetTools.UCFileStructures.Prim;

public interface IPrim
{
	public PrimPoint[] PrimPoints { get; set; }

	public PrimFace3[] PrimFace3s { get; set; }
	public PrimFace4[] PrimFace4s { get; set; }
}
