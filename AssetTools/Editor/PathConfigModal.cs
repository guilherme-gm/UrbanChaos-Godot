#if TOOLS

using Godot;
using static AssetTools.Editor.Constants;

namespace AssetTools.Editor;

[Tool]
public partial class PathConfigModal : Window
{
	[Export]
	private LineEdit UCFolder;

	[Export]
	private LineEdit WorkFolder;

	public override void _Ready() {
		var settings = EditorInterface.Singleton.GetEditorSettings();
		this.UCFolder.Text = settings.GetProjectMetadata(Config.Section, Config.UCPathKey, "").AsString();
		this.WorkFolder.Text = settings.GetProjectMetadata(Config.Section, Config.WorkPathKey, "").AsString();
	}

	public void OnSaveButtonPressed() {
		var settings = EditorInterface.Singleton.GetEditorSettings();
		settings.SetProjectMetadata(Config.Section, Config.UCPathKey, this.UCFolder.Text.Trim());
		settings.SetProjectMetadata(Config.Section, Config.WorkPathKey, this.WorkFolder.Text.Trim());

		this.QueueFree();
	}

	public void OnCancelButtonPressed() {
		this.QueueFree();
	}
}

#endif
