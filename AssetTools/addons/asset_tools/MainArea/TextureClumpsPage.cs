using AssetTools.AssetManagers;
using Godot;
using System.Collections.Generic;

[Tool]
public partial class TextureClumpsPage : VBoxContainer
{
	[Export]
	private Tree FileTree { get; set; }

	[Export]
	private Button ExtractAllButton { get; set; }

	[Export]
	private TextureRect TgaDisplay { get; set; }

	[Export]
	private LineEdit SearchTxt { get; set; }

	private Dictionary<string, List<string>> TreeData { get; set; }

	public override void _Ready() {
		this.ReloadTextureList();
		this.DrawFileTree();
	}

	public void ReloadTextureList() {
		this.TreeData = [];

		var clumpList = TextureManager.Instance.ListClumps();
		foreach (var clump in clumpList) {
			var files = new List<string>();
			this.TreeData.Add(clump, files);

			var clumpFiles = TextureManager.Instance.ListClumpFiles(clump);
			foreach (var clumpFile in clumpFiles) {
				files.Add(clumpFile);
			}
		}
	}

	private void DrawFileTree() {
		this.FileTree.Clear();

		var treeRoot = this.FileTree.CreateItem(null);
		foreach (var clump in this.TreeData.Keys) {
			var clumpNode = this.FileTree.CreateItem(treeRoot);
			clumpNode.SetText(0, clump);

			foreach (var clumpFile in this.TreeData[clump]) {
				var fileNode = this.FileTree.CreateItem(clumpNode);
				fileNode.SetText(0, clumpFile);
				fileNode.SetMetadata(0, $"{clump}|{clumpFile}");
			}

			clumpNode.Collapsed = true;
		}
	}

	private void DrawSearchTree(string query) {
		this.FileTree.Clear();

		var treeRoot = this.FileTree.CreateItem(null);
		foreach (var clump in this.TreeData.Keys) {
			foreach (var clumpFile in this.TreeData[clump]) {
				var label = $"{clump}/{clumpFile}";
				if (label.Contains(query)) {
					var fileNode = this.FileTree.CreateItem(treeRoot);
					fileNode.SetText(0, label);
					fileNode.SetMetadata(0, $"{clump}|{clumpFile}");
				}
			}
		}
	}

	public void ExtractAllClumps() {
		this.ExtractAllButton.Disabled = true;

		foreach (var clump in TextureManager.Instance.ListClumps()) {
			GD.Print($"Extracting {clump}...");
			TextureManager.Instance.ExtractClump(clump);
		}

		GD.Print("Clump extractiong completed.");
		this.ExtractAllButton.Disabled = false;

		this.ReloadTextureList();
	}

	public void OnRefreshBtnClicked() {
		this.ReloadTextureList();
		this.SearchTxt.Text = "";
		this.DrawFileTree();
	}

	public void OnSearchBtnClicked() {
		var query = this.SearchTxt.Text;

		if (query == "") {
			this.DrawFileTree();
		} else {
			this.DrawSearchTree(query);
		}
	}

	public void OnTreeItemSelected() {
		var item = this.FileTree.GetSelected();
		var meta = item.GetMetadata(0).AsString();

		if (meta == "") {
			GD.Print("Selected node is not a clump file.");
			this.TgaDisplay.Visible = false;
			return;
		}

		var parts = meta.Split('|');
		var clumpName = parts[0];
		var fileName = parts[1];

		if (fileName.ToLower().EndsWith(".tga")) {
			var image = new Image();
			var err = image.Load(TextureManager.GetWorkDirPath(clumpName, fileName));
			if (err != Error.Ok) {
				GD.PrintErr($"Failed to load image... {err}");
				return;
			}

			var texture = new ImageTexture();
			texture.SetImage(image);

			this.TgaDisplay.Texture = texture;
			this.TgaDisplay.Visible = true;
		} else {
			this.TgaDisplay.Visible = false;
		}
	}
}
