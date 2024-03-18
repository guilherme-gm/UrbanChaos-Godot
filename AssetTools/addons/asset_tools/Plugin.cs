#if TOOLS
using Godot;

namespace AssetTools.Addons.Asset_Tools;

[Tool]
public partial class Plugin : EditorPlugin
{
	private Control AssetMenuControl = null;

	private PackedScene MainPanel = ResourceLoader.Load<PackedScene>(Constants.Scenes.AssetToolMain);

	private Control MainPanelInstance;

	public override void _EnterTree() {
		this.AssetMenuControl = GD
			.Load<PackedScene>(Constants.Scenes.AssetToolsMenu)
			.Instantiate<Control>();

		this.AddControlToDock(DockSlot.LeftUr, this.AssetMenuControl);

		this.MainPanelInstance = this.MainPanel.Instantiate<Control>();
		EditorInterface.Singleton.GetEditorMainScreen().AddChild(this.MainPanelInstance);
		this._MakeVisible(false);
	}

	public override void _ExitTree() {
		this.RemoveControlFromDocks(this.AssetMenuControl);
		this.AssetMenuControl.Free();

		this.MainPanelInstance?.QueueFree();
	}

	public override bool _HasMainScreen() {
		return true;
	}

	public override void _MakeVisible(bool visible) {
		if (this.MainPanelInstance != null) {
			this.MainPanelInstance.Visible = visible;
		}
	}

	public override string _GetPluginName() {
		return "UC Assets";
	}

	public override Texture2D _GetPluginIcon() {
		return EditorInterface.Singleton.GetEditorTheme().GetIcon("Node", "EditorIcons");
	}
}
#endif
