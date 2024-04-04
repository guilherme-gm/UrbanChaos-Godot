using AssetTools.AssetManagers;
using AssetTools.UCWorld.Maps;
using Godot;

namespace AssetTools.Addons.Asset_Tools;

[Tool]
public partial class MissionsPage : VBoxContainer
{
	[Export]
	private Tree FileTree { get; set; }

	[Export]
	private LineEdit SearchTxt { get; set; }

	[Export]
	private Camera3D Camera { get; set; }

	[Export]
	private OptionButton TextureSetOptions { get; set; }

	[Export]
	private MissionTreeArea MissionTreeArea { get; set; }

	[Export]
	private MissionMeshInstance MissionMeshInstance { get; set; }

	[Export]
	private MapRenderer MapRenderer { get; set; }

	private string[] MapFilesList { get; set; }

	public override void _Ready() {
		this.ReloadMapsList();
		this.DrawFileTree();
	}

	public void OnRefreshBtnClicked() {
		this.ReloadMapsList();
		this.SearchTxt.Text = "";
		this.DrawFileTree();
	}

	public void ReloadMapsList() {
		this.LoadTextureSets();
		this.MapFilesList = MissionsManager.Instance.ListFiles();
	}

	private void LoadTextureSets() {
		this.TextureSetOptions.Clear();
		this.TextureSetOptions.AddItem("None", 0);
		this.TextureSetOptions.SetItemMetadata(0, "");

		int id = 1;
		var clumps = TextureManager.Instance.ListClumps();
		foreach (var clumpName in clumps) {
			this.TextureSetOptions.AddItem(clumpName, id);
			this.TextureSetOptions.SetItemMetadata(id, clumpName);
			id++;
		}
	}

	private void DrawFileTree(string query = "") {
		this.FileTree.Clear();

		var treeRoot = this.FileTree.CreateItem(null);
		foreach (var mapFile in this.MapFilesList) {
			if (query != "" && !mapFile.Contains(query)) {
				continue;
			}

			var mapNode = this.FileTree.CreateItem(treeRoot);
			mapNode.SetText(0, mapFile);
			mapNode.SetMetadata(0, mapFile);

			mapNode.SetCustomColor(0, Color.FromHtml("FEFEFE"));
		}
	}

	public void OnSearchBtnClicked() {
		var query = this.SearchTxt.Text;

		this.DrawFileTree(query);
	}

	public void OnTreeItemSelected() {
		var fileName = this.FileTree.GetSelected().GetMetadata(0).AsString();
		var mission = MissionsManager.Instance.LoadMission(fileName);
		var map = MapManager.Instance.LoadUCMap(mission.UcmFile.MapName);
		var clumpName = fileName.Replace(".ucm", ".txc");
		this.MapRenderer.SetMap(clumpName, map);
		this.MapRenderer.Visible = true;
		this.MissionTreeArea.SetMission(mission);
	}

#pragma warning disable IDE0060 // Remove unused parameter -- part of API
	public void OnTextureSetChanged(int option) {
		// @TODO:
		// this.Render();
	}
#pragma warning restore IDE0060 // Remove unused parameter
}
