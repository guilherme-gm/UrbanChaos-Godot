namespace AssetTools.UCFileStructures.Maps.SuperMap;

[Deserializer.DeserializeGenerator]
public partial class DFacet
{
	public byte FacetType { get; set; }
	public byte Height { get; set; }
	/** these are bytes because they are grid based  */
	[Deserializer.FixedArray(Dimensions = [2])]
	public byte[] X { get; set; }
	[Deserializer.FixedArray(Dimensions = [2])]
	public short[] Y { get; set; }
	/** these are bytes because they are grid based  */
	[Deserializer.FixedArray(Dimensions = [2])]
	public byte[] Z { get; set; }
	public ushort FacetFlags { get; set; }
	public ushort StyleIndex { get; set; }
	public ushort Building { get; set; }
	public ushort DStorey { get; set; }
	public byte FHeight { get; set; }
	public byte BlockHeight { get; set; }
	/** How open or closed a STOREY_TYPE_OUTSIDE_DOOR is. */
	public byte Open { get; set; }
	/** Index into NIGHT_dfcache[] or NULL... */
	public byte Dfcache { get; set; }
	/** When a fence has been hit hard by something. */
	public byte Shake { get; set; }
	public byte CutHole { get; set; }
	[Deserializer.FixedArray(Dimensions = [2])]
	public byte[] Counter { get; set; }

	partial void PostDeserialize() {
		this.Dfcache = 0; // After deserialization the original code zeores it.
	}
}
