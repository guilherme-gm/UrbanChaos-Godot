using Godot;

namespace Game;

public partial class Init : Control
{
	[Export]
	private Label StatusLbl { get; set; }

	public override void _Ready() {
		if (!ProjectSettings.LoadResourcePack("res://GameAssets.pck", true)) {
			this.StatusLbl.Text = $"Failed to load game assets. Make sure you have used the import tool first.";
			return;
		}

		_ = this.GetTree().CallDeferred("change_scene_to_file", "res://Scenes/MainMenu.tscn");
	}
}
