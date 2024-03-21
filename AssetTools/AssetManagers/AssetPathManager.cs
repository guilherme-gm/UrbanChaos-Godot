using Godot;
using System.IO;
using static AssetTools.Addons.Asset_Tools.Constants;

namespace AssetTools.AssetManagers;

public class AssetPathManager
{
	public static readonly AssetPathManager Instance = new AssetPathManager();

	public string GodotPath { get; private set; }

	public string UCFolderPath { get; private set; }

	public string WorkFolderPath { get; private set; }

	public string GameFolderPath { get; private set; }

	private AssetPathManager() {
		this.ReloadPath();
	}

	public void ReloadPath() {
#if TOOLS
		if (Engine.IsEditorHint()) {
			var settings = EditorInterface.Singleton.GetEditorSettings();
			this.UCFolderPath = settings.GetProjectMetadata(Config.Section, Config.UCPathKey, "").AsString();
			this.WorkFolderPath = settings.GetProjectMetadata(Config.Section, Config.WorkPathKey, "").AsString();
		}
#endif
	}

	public void SetPaths(string godotPath, string ucPath, string gamePath) {
		this.GodotPath = godotPath;
		this.UCFolderPath = ucPath;
		this.WorkFolderPath = Path.Join(gamePath, "Temp");
		_ = Directory.CreateDirectory(this.WorkFolderPath);
		this.GameFolderPath = gamePath;
	}
}
