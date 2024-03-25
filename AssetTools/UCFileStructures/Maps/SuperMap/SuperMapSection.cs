namespace AssetTools.UCFileStructures.Maps.SuperMap;

[Deserializer.DeserializeGenerator(AdditionalParams = [$"int {nameof(Iam.SaveType)}"])]
public partial class SuperMapSection
{
	public ushort DBuildingCount { get; set; }
	public ushort DFacetCount { get; set; }
	public ushort DStyleCount { get; set; }

	[Deserializer.Condition(When = $"{nameof(Iam.SaveType)} >= 17")]
	public ushort PaintMemCount { get; set; }

	[Deserializer.Condition(When = $"{nameof(Iam.SaveType)} >= 17")]
	public ushort DStoreyCount { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(DBuildingCount))]
	[Deserializer.Nested]
	public DBuilding[] DBuildings { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(DFacetCount))]
	[Deserializer.Nested]
	public DFacet[] DFacets { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(DStyleCount))]
	public ushort[] DStyles { get; set; }


	[Deserializer.Condition(When = $"{nameof(Iam.SaveType)} >= 17")]
	[Deserializer.VariableSizedArray(SizePropertyName = nameof(PaintMemCount))]
	public byte[] PaintMem { get; set; }

	[Deserializer.Condition(When = $"{nameof(Iam.SaveType)} >= 17")]
	[Deserializer.VariableSizedArray(SizePropertyName = nameof(DStoreyCount))]
	[Deserializer.Nested]
	public DStorey[] DStoreys { get; set; }

	[Deserializer.Condition(When = $"{nameof(Iam.SaveType)} >= 21")]
	[Deserializer.Nested]
	public InsidesSection InsidesSection { get; set; }

	[Deserializer.Nested]
	public WalkablesSection WalkablesSection { get; set; }

	[Deserializer.Condition(When = $"{nameof(Iam.SaveType)} >= 23")]
	[Deserializer.Nested]
	public MapObjectSection SupermapMapObjects { get; set; }
}
