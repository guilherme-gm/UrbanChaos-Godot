using AssetTools.AssetManagers;
using Godot;

namespace AssetTools.Addons.Asset_Tools;

[Tool]
public partial class MapsPage : VBoxContainer
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
	private MissionTree MapTreeView { get; set; }

	private string[] MapFilesList { get; set; }

	public override void _Ready() {
		this.ReloadMapsList();
		this.DrawFileTree();
	}

	public void OnRefreshBtnClicked() {
		this.ReloadMapsList();
		// this.SearchTxt.Text = "";
		// this.DrawFileTree();
	}

	public void ReloadMapsList() {
		// this.LoadTextureSets();
		this.MapFilesList = MapsManager.Instance.ListFiles();
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
		// var query = this.SearchTxt.Text;

		// this.DrawFileTree(query);
	}

	public void OnTreeItemSelected() {
		var fileName = this.FileTree.GetSelected().GetMetadata(0).AsString();
		var map = MapsManager.Instance.LoadMap(fileName);
		this.MapTreeView.SetMissoin(fileName, map);
	}

#pragma warning disable IDE0060 // Remove unused parameter -- part of API
	public void OnTextureSetChanged(int option) {
		// this.Render();
	}
#pragma warning restore IDE0060 // Remove unused parameter
}
