namespace AssetTools.UCFileStructures.MultiPrim;

[Deserializer.DeserializeGenerator]
public partial class BodyDef
{
	/// <summary>
	/// 1..14 is a normal person
	/// </summary>
	[Deserializer.FixedArray(Dimensions = [20])]
	public byte[] BodyParts { get; set; }
}
