namespace AssetTools.UCFileStructures.Maps;

[Deserializer.DeserializeGenerator]
public partial class MapThingPsxSection
{
	public ushort Count { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(Count))]
	[Deserializer.Nested]
	public MapThingPsx[] MapThingPsxes { get; set; }
}
