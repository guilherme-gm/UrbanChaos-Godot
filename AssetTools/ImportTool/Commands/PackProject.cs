using AssetTools.AssetManagers;
using Godot;
using System.IO;

namespace AssetTools.ImportTool.Commands;

public class PackProject : ICommand
{
	public void Execute() {
		var godotPath = AssetPathManager.Instance.GodotPath;
		var folderPath = AssetPathManager.Instance.WorkFolderPath;
		var outPath = Path.Join(AssetPathManager.Instance.GameFolderPath, "GameAssets.pck");

		_ = OS.Execute(
			godotPath,
			["--headless", "--path", folderPath, "--export-pack", "Windows Desktop", outPath]);
	}

	public string GetLog() {
		return "Packing assets...";
	}
}
