using AssetTools.Utils;

namespace AssetTools.UCFileStructures.Maps;

/// <summary>
/// The High Resolution Map
/// 
/// Originally called "PapHi", but according to developer notes, "Pap" is actually a new version of "Map".
/// </summary>
[Deserializer.DeserializeGenerator]
public partial class MapHi
{
	[Deserializer.Nested]
	public CompressedTextureInfo Texture { get; set; }

	/// <summary>
	/// Extra flags about how this map cell should behave.
	/// 
	/// Original code comment: "full but some sewer stuff that could go perhaps"
	/// </summary>
	[Deserializer.FlagsList(ReadStatement = "br.ReadUInt16()")]
	public Flags<MapFlag> Flags { get; set; }

	/// <summary>Compressed storage of Alt (expanded post deserialization into Altitude)</summary>
	public sbyte CompressedAltitude { get; set; }

	[Deserializer.Skip]
	public int Altitude { get; set; }

	/// <summary>
	/// padding; better find something to do with this 16K
	/// </summary>
	private sbyte CompressedHeight { get; set; }

	[Deserializer.Skip]
	public int Height { get; set; }

	partial void PostDeserialize() {
		this.Altitude = this.CompressedAltitude << 3;
		this.Height = this.CompressedHeight << 6;
	}
}
