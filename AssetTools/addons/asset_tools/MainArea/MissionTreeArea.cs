using AssetTools.Addons.Asset_Tools;
using AssetTools.Structures;
using Godot;

[Tool]
public partial class MissionTreeArea : VBoxContainer
{
	[Export]
	private MissionTree MissionTree { get; set; }

	[Export]
	private CheckBox DrawHighResCheck { get; set; }

	private Mission Mission { get; set; }

	private void OnDrawHighResToggleChanged(bool newVal) {
		this.MissionTree.SetMission(this.Mission, newVal);
	}

	public void SetMission(Mission mission) {
		this.Mission = mission;
		this.MissionTree.SetMission(mission, this.DrawHighResCheck.ButtonPressed);
	}
}
