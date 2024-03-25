namespace AssetTools.UCFileStructures.Maps;

[Deserializer.DeserializeGenerator]
public partial class MapObjectSection
{
	public int ObjectCount { get; set; }

	/** The objects */
	[Deserializer.VariableSizedArray(SizePropertyName = nameof(ObjectCount))]
	[Deserializer.Nested]
	public MapObject[] Objects { get; set; }

	/** index (11 bits) / num (5 bits) */
	[Deserializer.FixedArray(Dimensions = [32 * 32])]
	public ushort[] Mapwho { get; set; }
}
