using Godot;
using UCFileStructures.Mission;

namespace AssetTools.Addons.Asset_Tools;

[Tool]
public partial class MissionTree : Tree
{
	private Ucm Mission = null;

	private string FileName;

	public override void _Ready() {
		this.SetColumnTitle(0, "Name");
		this.SetColumnTitle(1, "Value");
	}

	public void SetMission(string fileName, Ucm mission) {
		this.FileName = fileName;
		this.Mission = mission;
		this.RedrawTree();
	}

	private TreeItem CreateItem(TreeItem parent, string name, string value) {
		var item = this.CreateItem(parent);
		item.SetText(0, $"{name}:");
		item.SetText(1, value);

		return item;
	}

	private void RedrawTree() {
		this.Clear();

		var treeRoot = this.CreateItem(null, "Mission (UCM)", this.FileName);
		_ = this.CreateItem(treeRoot, "Version", this.Mission.Version.ToString());
		_ = this.CreateItem(treeRoot, "Used", this.Mission.Used.ToString());
		_ = this.CreateItem(treeRoot, "MissionName", this.Mission.MissionName);
		_ = this.CreateItem(treeRoot, "MapName", this.Mission.MapName);
		_ = this.CreateItem(treeRoot, "BriefName", this.Mission.BriefName);
		_ = this.CreateItem(treeRoot, "LightMapName", this.Mission.LightMapName);
		_ = this.CreateItem(treeRoot, "CitSezName", this.Mission.CitSezName);
	}
}
