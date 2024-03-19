using AssetTools;
using AssetTools.AssetManagers;
using Godot;

[Tool]
public partial class PrimsPage : VBoxContainer
{
	[Export]
	private Tree FileTree { get; set; }

	[Export]
	private LineEdit SearchTxt { get; set; }

	[Export]
	private PrimMeshInstance PrimMesh { get; set; }

	[Export]
	private Camera3D Camera { get; set; }

	private PrimsManager.PrimFileInfo[] PrimFileInfo { get; set; }

	private Vector3 LookAtPos { get; set; }

	public override void _Ready() {
		this.ReloadTextureList();
		this.DrawFileTree();
	}

	public void ReloadTextureList() {
		this.PrimFileInfo = PrimsManager.Instance.ListPrims();
	}

	private void DrawFileTree(string query = "") {
		this.FileTree.Clear();

		var treeRoot = this.FileTree.CreateItem(null);
		foreach (var primFile in this.PrimFileInfo) {
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
		var boundingBox = this.PrimMesh.GetAabb();
		var longSize = boundingBox.Size.Y;
		this.LookAtPos = boundingBox.GetCenter();
		this.Camera.LookAtFromPosition(this.LookAtPos - (Vector3.Back * longSize * 2), this.LookAtPos);
	}

	public void OnTreeItemSelected() {
		var item = this.FileTree.GetSelected();
		var fileName = item.GetMetadata(0).AsString();

		if (fileName == "") {
			this.PrimMesh.Visible = false;
			return;
		}

		this.PrimMesh.LoadPrim(PrimsManager.GetUCPrimPath(fileName));
		this.PrimMesh.Visible = true;
		this.ResetView();
	}
}
