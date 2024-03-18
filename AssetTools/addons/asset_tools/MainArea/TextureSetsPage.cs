using AssetTools.AssetManagers;
using Godot;

[Tool]
public partial class TextureSetsPage : VBoxContainer
{
	[Export]
	private Tree FileTree { get; set; }

	public override void _Ready() {
		this.ReloadStylesList();
	}

	public void ReloadStylesList() {
		this.FileTree.Clear();
		var treeRoot = this.FileTree.CreateItem(null);

		var fileList = TmaManager.Instance.ListFiles();
		foreach (var file in fileList) {
			var fileNode = this.FileTree.CreateItem(treeRoot);
			fileNode.SetText(0, file);
		}
	}

	public void OnTreeItemSelected() {
		var item = this.FileTree.GetSelected();
		var name = item.GetText(0);

		var tma = TmaManager.Instance.LoadFile(name);
		foreach (var styleName in tma.TextureStyleNameSection.Names) {
			GD.Print(styleName);
		}

		// @TODO: What should we do with those?
		// var serializer = new JsonSerializer();
		// using var sw = new StreamWriter(@"test.json");
		// using JsonWriter writer = new JsonTextWriter(sw);
		// serializer.Serialize(writer, tma);

		// _ = ResourceSaver.Save(tma, "test.res");
		// var result = Json.Stringify(Variant.CreateFrom(tma));
		// GD.Print(result);
	}
}
