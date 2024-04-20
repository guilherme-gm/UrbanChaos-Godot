namespace AssetTools.UCFileStructures.MultiPrim;

// Note: MOJ files also goes here
[Deserializer.DeserializeGenerator]
public partial class MultiObject
{
	public int SaveType { get; set; }
	public int StartObj { get; set; }
	public int EndObj { get; set; }

	[Deserializer.Skip]
	public int ObjCount => this.EndObj - this.StartObj;

	// Maybe this cam be replaced with Prim
	[Deserializer.VariableSizedArray(SizePropertyName = nameof(ObjCount))]
	[Deserializer.Nested(AdditionalParams = ["value.SaveType"])]
	public MultiObjectPrim[] Objects { get; set; }
}
