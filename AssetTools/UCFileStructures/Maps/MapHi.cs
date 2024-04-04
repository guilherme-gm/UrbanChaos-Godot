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
	[Deserializer.CastVal(ReadStatement = "br.ReadUInt16()")]
	public CompressedTextureInfo Texture { get; set; }

	/// <summary>
	/// Extra flags about how this map cell should behave.
	/// 
	/// Original code comment: "full but some sewer stuff that could go perhaps"
	/// </summary>
	[Deserializer.FlagsList(ReadStatement = "br.ReadUInt16()")]
	public Flags<MapFlag> Flags { get; set; }

	[Deserializer.CastVal(ReadStatement = "br.ReadSByte()")]
	public HighResAltitude Alt { get; set; }

	/// <summary>
	/// padding; better find something to do with this 16K
	/// </summary>
	[Deserializer.CastVal(ReadStatement = "br.ReadSByte()")]
	public HighResHeight Height { get; set; }
}
