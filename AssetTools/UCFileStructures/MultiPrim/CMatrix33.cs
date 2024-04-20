namespace AssetTools.UCFileStructures.MultiPrim;

[Deserializer.DeserializeGenerator]
public partial class CMatrix33
{
	[Deserializer.FixedArray(Dimensions = [3])]
	public int[] M { get; set; }

	public override string ToString() {
		return $"CMatrix33({this.M[0]}, {this.M[1]}, {this.M[2]})";
	}
}
