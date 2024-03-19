namespace AssetTools.UCFileStructures.Prim;

[Deserializer.DeserializeGenerator]
public partial class PrimPoint
{
	public short X;
	public short Y;
	public short Z;

	public override string ToString() {
		return $"PrimPoint(  {this.X}  , {this.Y}  , {this.Z}  )";
	}
}
