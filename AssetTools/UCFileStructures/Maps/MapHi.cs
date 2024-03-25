namespace AssetTools.UCFileStructures.Maps;

/**
 * The High Resolution Map 
 *
 * Originally called "PapHi", but according to developer notes, "Pap" is actually a new version of "Map".
 */
[Deserializer.DeserializeGenerator]
public partial class MapHi
{
	/** 3 spare bits here */
	public ushort Texture { get; set; }

	/** full but some sewer stuff that could go perhaps */
	public ushort Flags { get; set; }
	public sbyte Alt { get; set; }

	/** //padding; // better find something to do with this 16K */
	public sbyte Height { get; set; }
}
