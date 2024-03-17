#if TOOLS
using Godot;

namespace AssetTools.Addons.Asset_Tools;

[Tool]
public partial class Plugin : EditorPlugin
{
	private Control AssetMenuControl = null;

	public override void _EnterTree() {
		this.AssetMenuControl = GD
			.Load<PackedScene>(Constants.Scenes.AssetToolsMenu)
			.Instantiate<Control>();
		this.AddControlToDock(DockSlot.LeftUr, this.AssetMenuControl);
	}

	public override void _ExitTree() {
		this.RemoveControlFromDocks(this.AssetMenuControl);
		this.AssetMenuControl.Free();
	}
}
#endif
