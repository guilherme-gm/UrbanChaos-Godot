#if TOOLS

using Godot;

namespace AssetTools.Editor;

[Tool]
public partial class PluginLoader : EditorPlugin
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
