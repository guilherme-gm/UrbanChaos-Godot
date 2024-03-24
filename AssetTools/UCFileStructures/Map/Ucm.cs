namespace UCFileStructures.Map;

[Deserializer.DeserializeGenerator]
public partial class Ucm
{
	public int Version { get; set; }

	public int Used { get; set; }

	[Deserializer.FixedLenString(Length = 260)]
	public string BriefName { get; set; }

	/**
	 * LGT file
	 * Data\Lighting\*.lgt
	 */
	[Deserializer.FixedLenString(Length = 260)]
	public string LightMapName { get; set; }

	/**
	 * IAM file
	 */
	[Deserializer.FixedLenString(Length = 260)]
	public string MapName { get; set; }

	[Deserializer.FixedLenString(Length = 260)]
	public string MissionName { get; set; }

	/**
	 * text/*.txt -- NPC speech text
	 */
	[Deserializer.FixedLenString(Length = 260)]
	public string CitSezName { get; set; }
}
