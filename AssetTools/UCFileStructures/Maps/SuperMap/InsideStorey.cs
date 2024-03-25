namespace AssetTools.UCFileStructures.Maps.SuperMap;

[Deserializer.DeserializeGenerator]
public partial class InsideStorey
{
	/** // bounding rectangle of floor */
	public byte MinX { get; set; }
	public byte MinZ { get; set; }
	public byte MaxX { get; set; }
	public byte MaxZ { get; set; }
	/** // index into inside_block (block of data of size bounding rect) data is room numbers 1..15 top 4 bits reserved */
	public ushort InsideBlock { get; set; }
	/** // link list of stair structures for this floor */
	public ushort StairCaseHead { get; set; }
	/** // Inside style to use for floor */
	public ushort TexType { get; set; }
	/** // index into facets that make up this building */
	public ushort FacetStart { get; set; }
	/** // Facet After last used facet for inside the floor */
	public ushort FacetEnd { get; set; }
	/** // Y co-ord could come in handy */
	public short StoreyY { get; set; }
	public ushort Building { get; set; }
	[Deserializer.FixedArray(Dimensions = [2])]
	public ushort[] Dummy { get; set; }
}
