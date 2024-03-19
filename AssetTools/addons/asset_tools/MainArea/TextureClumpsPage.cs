using AssetTools.AssetManagers;
using Godot;

[Tool]
public partial class TextureClumpsPage : VBoxContainer
{
	[Export]
	private Tree FileTree { get; set; }

	[Export]
	private Button ExtractAllButton { get; set; }

	[Export]
	private TextureRect TgaDisplay { get; set; }

	public override void _Ready() {
		this.ReloadTextureList();
	}

	public void ReloadTextureList() {
		this.FileTree.Clear();
		var treeRoot = this.FileTree.CreateItem(null);

		var clumpList = TextureManager.Instance.ListClumps();
		foreach (var clump in clumpList) {
			var clumpNode = this.FileTree.CreateItem(treeRoot);
			clumpNode.SetText(0, clump);

			var clumpFiles = TextureManager.Instance.ListClumpFiles(clump);
			foreach (var clumpFile in clumpFiles) {
				var fileNode = this.FileTree.CreateItem(clumpNode);
				fileNode.SetText(0, clumpFile);
				fileNode.SetMetadata(0, $"{clump}|{clumpFile}");
			}

			clumpNode.Collapsed = true;
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
