using Godot;
using static AssetTools.Addons.Asset_Tools.Constants;

namespace AssetTools.AssetManagers;

public class AssetPathManager
{
	public static readonly AssetPathManager Instance = new AssetPathManager();

	public string UCPath { get; private set; }

	public string WorkFolderPath { get; private set; }

	private AssetPathManager() {
		this.ReloadPath();
	}

	public void ReloadPath() {
		var settings = EditorInterface.Singleton.GetEditorSettings();
		this.UCPath = settings.GetProjectMetadata(Config.Section, Config.UCPathKey, "").AsString();
		this.WorkFolderPath = settings.GetProjectMetadata(Config.Section, Config.WorkPathKey, "").AsString();
	}
}
