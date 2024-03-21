using AssetTools.AssetManagers;
using System.IO;

namespace AssetTools.ImportTool.Commands;

public class DeleteTemp : ICommand
{
	public void Execute() {
		var folderPath = AssetPathManager.Instance.WorkFolderPath;
		Directory.Delete(folderPath, true);
	}

	public string GetLog() {
		return "Deleting temporary data...";
	}
}
