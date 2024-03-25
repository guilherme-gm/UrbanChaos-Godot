using UCFileStructures.Mission;

namespace AssetTools.Structures;

public class Mission
{
	public AssetLoadStatus UcmStatus { get; set; } = AssetLoadStatus.NotLoaded;
	public string UcmFilePath { get; set; } = "";
	public Ucm UcmFile { get; set; } = null;

	public Map Map { get; set; } = null;

	public int Version => this.UcmFile.Version;
	public int Used => this.UcmFile.Used;
	public string MissionName => this.UcmFile.MissionName;

	public string BriefFileName => this.UcmFile.BriefName;
	public string LightMapFileName => this.UcmFile.LightMapName;
	public string CitSezFileName => this.UcmFile.CitSezName;
}
