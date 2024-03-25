namespace AssetTools.UCFileStructures.Maps.SuperMap;

[Deserializer.DeserializeGenerator]
public partial class WalkablesSection
{
	public ushort DWalkableCount { get; set; }

	public ushort RoofFace4Count { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(DWalkableCount))]
	[Deserializer.Nested]
	public DWalkable[] DWalkables { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(RoofFace4Count))]
	[Deserializer.Nested]
	public RoofFace4[] RoofFace4s { get; set; }
}
