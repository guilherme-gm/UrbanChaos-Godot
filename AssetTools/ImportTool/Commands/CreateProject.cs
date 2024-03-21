using AssetTools.AssetManagers;
using System;
using System.IO;

namespace AssetTools.ImportTool.Commands;

public class CreateProject : ICommand
{
	public void Execute() {
		var folderPath = AssetPathManager.Instance.WorkFolderPath;

		if (Directory.Exists(folderPath) && Directory.GetFileSystemEntries(folderPath).Length > 0) {
			throw new Exception($"The temporary folder already exists at \"${folderPath}\". Maybe a previous conversion failed? You must delete it before continuing.");
		}

		_ = Directory.CreateDirectory(folderPath);
		File.WriteAllText(Path.Join(folderPath, "project.godot"), "");

		var presetContent = Godot.FileAccess
			.Open("res://ImportProjectPreset.txt", Godot.FileAccess.ModeFlags.Read)
			.GetAsText();
		File.WriteAllText(Path.Join(folderPath, "export_presets.cfg"), presetContent);
	}

	public string GetLog() {
		return "Creating project...";
	}
}
