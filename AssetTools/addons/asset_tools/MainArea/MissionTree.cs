using AssetTools.Structures;
using Godot;

namespace AssetTools.Addons.Asset_Tools;

[Tool]
public partial class MissionTree : Tree
{
	private Mission Mission = null;

	public override void _Ready() {
		this.SetColumnTitle(0, "Name");
		this.SetColumnTitle(1, "Value");
	}

	public void SetMission(Mission mission) {
		this.Mission = mission;
		this.RedrawTree();
	}

	private TreeItem CreateItem(TreeItem parent, string name, string value) {
		var item = this.CreateItem(parent);
		item.SetText(0, name);
		item.SetText(1, value);

		return item;
	}

	private TreeItem CreateItem(TreeItem parent, string name, string value, AssetLoadStatus status) {
		var item = this.CreateItem(parent);
		item.SetText(0, name);
		item.SetText(1, $"{value} ({status.Description})");
		item.SetCustomColor(1, Color.FromHtml(status.ColorHex));

		return item;
	}

	private void RedrawTree() {
		this.Clear();

		var treeRoot = this.CreateItem(null, "Mission (UCM)", this.Mission.UcmFilePath, this.Mission.UcmStatus);
		_ = this.CreateItem(treeRoot, "Version", this.Mission.Version.ToString());
		_ = this.CreateItem(treeRoot, "Used", this.Mission.Used.ToString());
		_ = this.CreateItem(treeRoot, "MissionName", this.Mission.MissionName);
		_ = this.CreateItem(treeRoot, "Map file (IAM)", this.Mission.Map.IamFilePath, this.Mission.Map.IamStatus);
		_ = this.CreateItem(treeRoot, "BriefName", this.Mission.BriefFileName);
		_ = this.CreateItem(treeRoot, "LightMapName", this.Mission.LightMapFileName);
		_ = this.CreateItem(treeRoot, "CitSezName", this.Mission.CitSezFileName);
	}
}
