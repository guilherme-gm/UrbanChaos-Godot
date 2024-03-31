using AssetTools.UCFileStructures.Map;

namespace AssetTools.UCFileStructures.Maps;

[Deserializer.DeserializeGenerator]
public partial class MapObjectSection
{
	public int ObjectCount { get; set; }

	/** The objects */
	[Deserializer.VariableSizedArray(SizePropertyName = nameof(ObjectCount))]
	[Deserializer.Nested]
	public MapObject[] Objects { get; set; }

	[Deserializer.FixedArray(Dimensions = [32, 32])]
	[Deserializer.CastVal(ReadStatement = "br.ReadUInt16()")]
	public Mapwho[][] Mapwho { get; set; }
}
