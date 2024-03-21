using AssetTools.UCFileStructures.Tma;
using System.IO;
using System.Linq;

namespace AssetTools.AssetManagers;

public class TmaManager
{
	public static readonly TmaManager Instance = new TmaManager();

	public string[] ListFiles() {
		var folderPath = Path.Join(AssetPathManager.Instance.UCFolderPath, "server/textures");
		var fileList = Directory
			.GetFiles(folderPath, "*.tma", SearchOption.AllDirectories)
			.Select((file) => Path.GetRelativePath(folderPath, file))
			.ToArray();

		return fileList;
	}

	public StyleTma LoadFile(string path) {
		var filePath = Path.Join(AssetPathManager.Instance.UCFolderPath, "server/textures", path);
		using var fs = new FileStream(filePath, FileMode.Open);
		using var br = new BinaryReader(fs);

		return StyleTma.Deserialize(br);
	}
}
