#if TOOLS

using Godot;

namespace AssetTools.Addons.Asset_Tools;

[Tool]
public partial class AssetToolsMenu : VBoxContainer
{
	public static void OnSettingsBtnPressed() {
		var menu = GD
			.Load<PackedScene>(Constants.Scenes.PathConfigModal)
			.Instantiate<PopupPanel>();

		EditorInterface.Singleton.PopupDialogCentered(menu);
	}
}

#endif
