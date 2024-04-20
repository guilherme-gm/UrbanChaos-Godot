using AssetTools.AssetManagers;
using AssetTools.Scenes.Renderers;
using Godot;

[Tool]
public partial class ThingPage : VBoxContainer
{
	[Export]
	private Tree FileTree { get; set; }

	[Export]
	private LineEdit SearchTxt { get; set; }

	// [Export]
	// private PrimMeshInstance PrimMesh { get; set; }

	[Export]
	private Camera3D Camera { get; set; }

	[Export]
	private OptionButton TextureSetOptions { get; set; }

	[Export]
	private AllFileTree AllFileTree { get; set; }

	[Export]
	private Node3D RenderRoot { get; set; }

	private ThingsManager.ThingFileInfo[] ThingFileInfo { get; set; }

	private Vector3 LookAtPos { get; set; }

	public override void _Ready() {
		this.ReloadTextureList();
		this.DrawFileTree();
	}

	public void ReloadTextureList() {
		this.LoadTextureSets();
		this.ThingFileInfo = ThingsManager.Instance.ListThings();
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
		foreach (var primFile in this.ThingFileInfo) {
			if (query != "" && !primFile.FileName.Contains(query)) {
				continue;
			}

			var primNode = this.FileTree.CreateItem(treeRoot);
			primNode.SetText(0, primFile.FileName);
			primNode.SetMetadata(0, primFile.FileName);

			if (primFile.IsIgnored) {
				primNode.SetCustomColor(0, Color.FromHtml("666666"));
			} else {
				primNode.SetCustomColor(0, Color.FromHtml("FEFEFE"));
			}
		}
	}

	public void OnRefreshBtnClicked() {
		this.ReloadTextureList();
		this.SearchTxt.Text = "";
		this.DrawFileTree();
	}

	public void OnSearchBtnClicked() {
		var query = this.SearchTxt.Text;

		this.DrawFileTree(query);
	}

	public void ResetView() {
		// var boundingBox = this.PrimMesh.GetAabb();
		// var longSize = boundingBox.Size.Y;
		// this.LookAtPos = boundingBox.GetCenter();
		// this.Camera.LookAtFromPosition(this.LookAtPos - (Vector3.Back * longSize * 2), this.LookAtPos);
	}

	private void Render() {
		var item = this.FileTree.GetSelected();
		var fileName = item.GetMetadata(0).AsString();

		if (fileName == "") {
			// this.PrimMesh.Visible = false;
			return;
		}

		// var textureSet = this.TextureSetOptions.GetSelectedMetadata().AsString();
		// this.PrimMesh.LoadPrim(ThingsManager.GetUCThingPath(fileName), textureSet);
		// this.PrimMesh.Visible = true;
		// this.ResetView();
	}

	private void LoadThingFile() {
		var name = this.FileTree.GetSelected().GetMetadata(0).AsString();
		var thing = ThingsManager.Instance.LoadThing(name);

		this.AllFileTree.SetAllFile(thing.All.Data);
		foreach (var child in this.RenderRoot.GetChildren()) {
			child.QueueFree();
		}

		foreach (var bodyPart in thing.BodyParts) {
			var renderer = new PolyListRenderer();
			renderer.SetPolyList("testdrive1a.txc", bodyPart.Faces);
			renderer.Position = bodyPart.Offset;
			renderer.Quaternion = bodyPart.Rotation;
			this.RenderRoot.AddChild(renderer);
		}
		GD.Print(thing.ToString());
	}

	public void OnTreeItemSelected() {
		this.LoadThingFile();
		this.Render();
	}

#pragma warning disable IDE0060 // Remove unused parameter -- part of API
	public void OnTextureSetChanged(int option) {
		this.Render();
	}
#pragma warning restore IDE0060 // Remove unused parameter
}
