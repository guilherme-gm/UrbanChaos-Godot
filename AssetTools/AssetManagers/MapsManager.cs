using System.IO;
using System.Linq;
using UCFileStructures.Map;

namespace AssetTools.AssetManagers;

public class MapsManager
{
	public static readonly MapsManager Instance = new MapsManager();

	public static string GetUCMapPath(string mapFileName = "") {
		return Path.Join(AssetPathManager.Instance.UCFolderPath, "levels", mapFileName);
	}

	public string[] ListFiles() {
		var folderPath = GetUCMapPath();
		var fileList = Directory
			.GetFiles(folderPath, "*.ucm", SearchOption.AllDirectories)
			.Select((file) => Path.GetRelativePath(folderPath, file))
			.ToArray();

		return fileList;
	}

	public Ucm LoadMap(string mapFileName) {
		var filePath = GetUCMapPath(mapFileName);
		using var fs = new FileStream(filePath, FileMode.Open);
		using var br = new BinaryReader(fs);
		var map = Ucm.Deserialize(br);

		return map;
	}
}
