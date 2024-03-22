using System.IO;
using System.Linq;

namespace AssetTools.AssetManagers;

public class MapsManager
{
	public static readonly MapsManager Instance = new MapsManager();

	public string[] ListFiles() {
		var folderPath = Path.Join(AssetPathManager.Instance.UCFolderPath, "levels");
		var fileList = Directory
			.GetFiles(folderPath, "*.ucm", SearchOption.AllDirectories)
			.Select((file) => Path.GetRelativePath(folderPath, file))
			.ToArray();

		return fileList;
	}
}
