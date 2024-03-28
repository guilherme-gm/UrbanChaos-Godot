namespace AssetTools.UCFileStructures.Maps;

/** The objects -- Originally Ob_ob */
[Deserializer.DeserializeGenerator]
public partial class MapObject
{
	public short Y { get; set; }
	private byte CompressedX { get; set; }

	/// <summary>
	/// X local to the current Square. MapWho's X should be combined with this.
	/// </summary>
	[Deserializer.Skip]
	public int LocalX { get; set; }

	private byte CompressedZ { get; set; }

	/// <summary>
	/// Z local to the current Square. MapWho's Z should be combined with this.
	/// </summary>
	[Deserializer.Skip]
	public int LocalZ { get; set; }
	public byte Prim { get; set; }
	private byte CompressedYaw { get; set; }
	[Deserializer.Skip]
	public int Yaw { get; set; }
	public byte Flags { get; set; }
	public byte InsideIndex { get; set; }

	partial void PostDeserialize() {
		this.LocalX = this.CompressedX << 2;
		this.LocalZ = this.CompressedZ << 2;
		this.Yaw = this.CompressedYaw << 3;
	}
}
