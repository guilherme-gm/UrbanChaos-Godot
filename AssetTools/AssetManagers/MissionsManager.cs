using System.IO;
using System.Linq;
using UCFileStructures.Mission;

namespace AssetTools.AssetManagers;

public class MissionsManager
{
	public static readonly MissionsManager Instance = new MissionsManager();

	public static string GetUCMissionPath(string missionFileName = "") {
		return Path.Join(AssetPathManager.Instance.UCFolderPath, "levels", missionFileName);
	}

	public string[] ListFiles() {
		var folderPath = GetUCMissionPath();
		var fileList = Directory
			.GetFiles(folderPath, "*.ucm", SearchOption.AllDirectories)
			.Select((file) => Path.GetRelativePath(folderPath, file))
			.ToArray();

		return fileList;
	}

	public Ucm LoadMissoin(string missionFileName) {
		var filePath = GetUCMissionPath(missionFileName);
		using var fs = new FileStream(filePath, FileMode.Open);
		using var br = new BinaryReader(fs);
		var map = Ucm.Deserialize(br);

		return map;
	}
}
