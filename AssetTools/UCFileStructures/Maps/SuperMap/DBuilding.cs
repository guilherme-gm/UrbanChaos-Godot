namespace AssetTools.UCFileStructures.Maps.SuperMap;

[Deserializer.DeserializeGenerator]
public partial class DBuilding
{
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
	public ushort StartFacet { get; set; }
	public ushort EndFacet { get; set; }
	public ushort Walkable { get; set; }
	[Deserializer.FixedArray(Dimensions = [2])]
	public byte[] Counter { get; set; }
	public ushort Padding { get; set; }
	/** If this building is a warehouse, this is an index into the WARE_ware[] array */
	public byte Ware { get; set; }

	[Deserializer.CastVal(ReadStatement = "br.ReadByte()")]
	public BuildingType Type { get; set; }
}
