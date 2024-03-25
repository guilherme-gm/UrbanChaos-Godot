namespace AssetTools.UCFileStructures.Maps.SuperMap;

[Deserializer.DeserializeGenerator]
public partial class DWalkable
{
	/* // Unused nowadays */
	public ushort StartPoint { get; set; }
	/* // Unused nowadays */
	public ushort EndPoint { get; set; }
	/* // Unused nowadays */
	public ushort StartFace3 { get; set; }
	/* // Unused nowadays */
	public ushort EndFace3 { get; set; }

	/* // These are indices into the roof faces */
	public ushort StartFace4 { get; set; }
	public ushort EndFace4 { get; set; }

	public byte X1 { get; set; }
	public byte Z1 { get; set; }
	public byte X2 { get; set; }
	public byte Z2 { get; set; }
	public byte Y { get; set; }
	public byte StoreyY { get; set; }
	public ushort Next { get; set; }
	public ushort Building { get; set; }
}
