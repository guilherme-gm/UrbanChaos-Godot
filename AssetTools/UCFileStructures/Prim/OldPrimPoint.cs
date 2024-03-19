namespace AssetTools.UCFileStructures.Prim;

[Deserializer.DeserializeGenerator]
public partial class OldPrimPoint
{
	public long X;
	public long Y;
	public long Z;

	public override string ToString() {
		return $"OldPrimPoint(  {this.X}  , {this.Y}  , {this.Z}  )";
	}
}
