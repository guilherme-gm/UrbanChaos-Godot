using AssetTools.UCFileStructures;
using System.IO;
using System.Linq;

namespace AssetTools.AssetManagers;

public class TextureManager
{
	public static readonly TextureManager Instance = new TextureManager();

	public static string GetUCClumpPath(string clumpName = "") {
		return Path.Join(AssetPathManager.Instance.UCPath, "clumps", clumpName);
	}

	public static string GetWorkDirPath(string clumpName = "", string fileName = "") {
		return Path.Join(AssetPathManager.Instance.WorkFolderPath, "clumps", clumpName, fileName);
	}

	public string[] ListClumps() {
		var folderPath = GetUCClumpPath();
		var fileList = Directory
			.GetFiles(folderPath, "*.txc", SearchOption.AllDirectories)
			.Select((file) => Path.GetRelativePath(folderPath, file))
			.ToArray();

		return fileList;
	}

	public void ExtractClump(string clumpName) {
		var filePath = GetUCClumpPath(clumpName);
		var outPath = GetWorkDirPath(clumpName);

		_ = Directory.CreateDirectory(outPath);

		using var fs = new FileStream(filePath, FileMode.Open);
		using var clumpBr = new BinaryReader(fs);

		var clump = FileClump.Deserialize(clumpBr);
		for (int i = 0; i < clump.Files.Length; i++) {
			var fileBuffer = clump.GetFile(i);
			if (fileBuffer == null) {
				continue;
			}

			string fileName;
			byte[] outBuffer;
			if (i <= 1567) {
				fileName = $"tex{i.ToString().PadLeft(3, '0')}.tga";

				using var ms = new MemoryStream(fileBuffer);
				using var fileBr = new BinaryReader(ms);
				var tgaFile = Tga.Deserialize(fileBr);
				outBuffer = tgaFile.Serialize();
			} else {
				fileName = $"sex{i.ToString().PadLeft(3, '0')}hi.sex";
				outBuffer = fileBuffer;
			}

			File.WriteAllBytes(Path.Join(outPath, fileName), outBuffer);
		}
	}

	public string[] ListClumpFiles(string clumpName) {
		var folderPath = GetWorkDirPath(clumpName);
		if (!Directory.Exists(folderPath)) {
			return [];
		}

		var fileList = Directory
			.GetFiles(folderPath, "", SearchOption.AllDirectories)
			.Select((file) => Path.GetRelativePath(folderPath, file))
			.ToArray();

		return fileList;
	}
}
