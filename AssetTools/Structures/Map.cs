using AssetTools.UCFileStructures.Maps;

namespace AssetTools.Structures;

public class Map
{
	public AssetLoadStatus IamStatus { get; set; } = AssetLoadStatus.NotLoaded;
	public string IamFilePath { get; set; } = "";
	public Iam IamFile { get; set; } = null;
}
