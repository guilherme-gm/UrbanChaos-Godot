namespace AssetTools.UCFileStructures.Maps.SuperMap;

[Deserializer.DeserializeGenerator]
public partial class InsidesSection
{
	public ushort InsideStoreyCount { get; set; }

	public ushort InsideStairCount { get; set; }

	public int InsideBlockCount { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(InsideStoreyCount))]
	[Deserializer.Nested]
	public InsideStorey[] InsideStoreys { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(InsideStairCount))]
	[Deserializer.Nested]
	public StairCase[] InsideStairs { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(InsideBlockCount))]
	public byte[] InsideBlocks { get; set; }
}
