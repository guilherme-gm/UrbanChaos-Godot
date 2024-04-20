using AssetTools.UCFileStructures.MultiPrim;
using AssetTools.UCFileStructures.Prim;
using Godot;
using System;
using System.Linq;

[Tool]
public partial class AllFileTree : Tree
{
	private AllFile AllFile = null;

	private TreeItem CreateItem(TreeItem parent, string name, string value) {
		var item = this.CreateItem(parent);
		item.SetText(0, name);
		item.SetText(1, value);

		return item;
	}

	private void DrawArrayNode<T>(TreeItem mapNode, string title, T[] entries, Func<T, string> valueFn) {
		var rootNode = this.CreateItem(mapNode, title, $"{entries.Length} items");
		rootNode.Collapsed = true;

		for (int i = 0; i < entries.Length; i++) {
			var item = entries[i];

			_ = this.CreateItem(
				rootNode,
				$"Item {i}",
				valueFn(item)
			);
		}
	}

	private void AddFacesNode(TreeItem facesListNode, PrimFace[] faces) {
		var idx = 0;

		foreach (var face in faces) {
			var faceNode = this.CreateItem(facesListNode, $"Face {idx}", "");
			idx++;

			if (face is PrimFace3 face3) {
				_ = this.CreateItem(faceNode, "Compressed Texture Page", face3.CompressedTexturePage.ToString());
				_ = this.CreateItem(faceNode, "DrawFlags", face3.DrawFlags.ToString());
				_ = this.CreateItem(faceNode, "Points", face3.Points.Select(v => v.ToString()).ToArray().Join(", "));
				// _ = this.CreateItem(faceNode, "Compessed UV", face3.CompressedUV.ToString());
				_ = this.CreateItem(faceNode, "Bright", face3.Bright.Select(v => v.ToString()).ToArray().Join(", ").ToString());
				_ = this.CreateItem(faceNode, "ThingIndex", face3.ThingIndex.ToString());
				_ = this.CreateItem(faceNode, "Col2", face3.Col2.ToString());
				_ = this.CreateItem(faceNode, "FaceFlags", face3.FaceFlags.ToString());
				_ = this.CreateItem(faceNode, "Type", face3.Type.ToString());
				_ = this.CreateItem(faceNode, "ID", face3.ID.ToString());
			} else if (face is PrimFace4 face4) {
				_ = this.CreateItem(faceNode, "Compressed Texture Page", face4.CompressedTexturePage.ToString());
				_ = this.CreateItem(faceNode, "DrawFlags", face4.DrawFlags.ToString());
				_ = this.CreateItem(faceNode, "Points", face4.Points.Select(v => v.ToString()).ToArray().Join(", "));
				// _ = this.CreateItem(faceNode, "Compessed UV", face4.CompressedUV.ToString());
				_ = this.CreateItem(faceNode, "Bright", face4.Bright.Select(v => v.ToString()).ToArray().Join(", ").ToString());
				_ = this.CreateItem(faceNode, "ThingIndex", face4.ThingIndex.ToString());
				_ = this.CreateItem(faceNode, "Col2", face4.Col2.ToString());
				_ = this.CreateItem(faceNode, "FaceFlags", face4.FaceFlags.ToString());
				_ = this.CreateItem(faceNode, "Type", face4.Type.ToString());
				_ = this.CreateItem(faceNode, "ID", face4.ID.ToString());
			}
		}
	}

	private void DrawGameKeyFrameElement(TreeItem root, GameKeyFrameElement element) {
		if (element == null) {
			return;
		}

		_ = this.CreateItem(root, "CMatrix", element.CMatrix.ToString());
		_ = this.CreateItem(root, "OffsetX", element.OffsetX.ToString());
		_ = this.CreateItem(root, "OffsetY", element.OffsetY.ToString());
		_ = this.CreateItem(root, "OffsetZ", element.OffsetZ.ToString());
		_ = this.CreateItem(root, "Pad", element.Pad.ToString());
	}

	private void DrawGameFightCol(TreeItem root, GameFightCol element) {
		if (element == null) {
			return;
		}

		_ = this.CreateItem(root, "Dist1", element.Dist1.ToString());
		_ = this.CreateItem(root, "Dist2", element.Dist2.ToString());
		_ = this.CreateItem(root, "Angle", element.Angle.ToString());
		_ = this.CreateItem(root, "Priority", element.Priority.ToString());
		_ = this.CreateItem(root, "Damage", element.Damage.ToString());
		_ = this.CreateItem(root, "Tween", element.Tween.ToString());
		_ = this.CreateItem(root, "AngleHitFrom", element.AngleHitFrom.ToString());
		_ = this.CreateItem(root, "Height", element.Height.ToString());
		_ = this.CreateItem(root, "Width", element.Width.ToString());
		_ = this.CreateItem(root, "Dummy", element.Dummy.ToString());
		_ = this.CreateItem(root, "NextIdx", element.NextIdx.ToString());
	}

	private void DrawGameKeyFrame(TreeItem root, GameKeyFrame frame) {
		if (frame == null) {
			return;
		}

		_ = this.CreateItem(root, "XYZIndex", frame.XYZIndex.ToString());
		_ = this.CreateItem(root, "TweenStep", frame.TweenStep.ToString());
		_ = this.CreateItem(root, "Flags", frame.Flags.ToString());
		var firstEleNode = this.CreateItem(root, "FirstElement", frame.FirstElementIdx.ToString());
		this.DrawGameKeyFrameElement(firstEleNode, frame.FirstElement);
		_ = this.CreateItem(root, "PrevFrameIdx", frame.PrevFrameIdx.ToString());
		// _ = this.CreateItem(node, "PrevFrame", item.PrevFrame.ToString());
		_ = this.CreateItem(root, "NextFrameIdx", frame.NextFrameIdx.ToString());
		// _ = this.CreateItem(node, "NextFrame", item.NextFrame.ToString());
		var fightNode = this.CreateItem(root, "Fight", frame.FightIdx.ToString());
		this.DrawGameFightCol(fightNode, frame.Fight);
	}

	private void DrawAnimKeyFrames(TreeItem gameChunkNode) {
		var animKeyFrames = this.AllFile.GameChunk.AnimKeyFrames;
		var animKeyFramesNode = this.CreateItem(gameChunkNode, "AnimKeyFrames", $"{animKeyFrames.Length} items");

		int idx = 0;
		foreach (var item in animKeyFrames) {
			idx++;
			var node = this.CreateItem(animKeyFramesNode, $"Item {idx}", "");
			this.DrawGameKeyFrame(node, item);

			if (idx == 50)
				break;
		}
	}

	private void DrawGameChunk(TreeItem treeRoot) {
		var gameChunk = this.AllFile.GameChunk;
		var gameChunkNode = this.CreateItem(treeRoot, "GameChunk", "");

		_ = this.CreateItem(gameChunkNode, "SaveType", gameChunk.SaveType.ToString());
		_ = this.CreateItem(gameChunkNode, "ElementCount", gameChunk.ElementCount.ToString());
		this.DrawArrayNode(gameChunkNode, "PeopleTypes", gameChunk.PeopleTypes,
			v => v.BodyParts.Select(b => b.ToString())
				.ToArray()
				.Join(", ")
				.ToString()
		);
		_ = this.CreateItem(gameChunkNode, "Addr1", gameChunk.Addr1.ToString());
		this.DrawAnimKeyFrames(gameChunkNode);


		_ = this.CreateItem(gameChunkNode, "Addr2", gameChunk.Addr2.ToString());
		var elementsNode = this.CreateItem(gameChunkNode, "The Element", $"{gameChunk.TheElements.Length} items");
		int idx = 0;
		foreach (var item in gameChunk.TheElements) {
			var node = this.CreateItem(elementsNode, $"Item {idx}", "");
			this.DrawGameKeyFrameElement(node, item);
			idx++;

			if (idx == 50)
				break;
		}
		var animListNode = this.CreateItem(gameChunkNode, "AnimList", $"{gameChunk.AnimList.Length} items");
		idx = 0;
		foreach (var item in gameChunk.AnimList) {
			var node = this.CreateItem(elementsNode, $"Item {idx}", "");
			this.DrawGameKeyFrame(node, item);
			idx++;

			if (idx == 50)
				break;
		}

		_ = this.CreateItem(gameChunkNode, "Addr3", gameChunk.Addr3.ToString());
		var fightColsNode = this.CreateItem(gameChunkNode, "FightCols", $"{gameChunk.FightCols.Length} items");
		idx = 0;
		foreach (var item in gameChunk.FightCols) {
			var node = this.CreateItem(elementsNode, $"Item {idx}", "");
			this.DrawGameFightCol(node, item);
			idx++;
			if (idx == 50)
				break;
		}

	}

	private void RedrawTree() {
		this.Clear();

		if (this.AllFile == null) {
			return;
		}

		var treeRoot = this.CreateItem(null, "All File", "");
		_ = this.CreateItem(treeRoot, "SaveType", this.AllFile.SaveType.ToString());

		var multiObjectListNode = this.CreateItem(treeRoot, "MultiObjects", $"{this.AllFile.Count} items");
		int multiObjectIdx = 0;
		// foreach (var multiObject in this.AllFile.MultiObjects) {
		var multiObject = this.AllFile.MultiObjects[0];
		{
			var multiObjectNode = this.CreateItem(multiObjectListNode, $"MultiObject {multiObjectIdx}", $"{multiObject.ObjCount} items");
			multiObjectIdx++;

			int objectIdx = 0;
			foreach (var obj in multiObject.Objects) {
				var objNode = this.CreateItem(multiObjectNode, $"Object {objectIdx}", "");
				objectIdx++;

				_ = this.CreateItem(objNode, "Name", obj.Name.ToString());

				var pointIdx = 0;
				var pointsNode = this.CreateItem(objNode, "Points", $"{obj.PointsCount} items");
				foreach (var point in obj.PrimPoints) {
					_ = this.CreateItem(pointsNode, $"Point {pointIdx}", point.ToString());
					pointIdx++;
				}

				var faces3Node = this.CreateItem(objNode, "Faces3", $"{obj.Faces3Count} items");
				this.AddFacesNode(faces3Node, obj.PrimFace3s);

				var faces4Node = this.CreateItem(objNode, "Faces4", $"{obj.Faces4Count} items");
				this.AddFacesNode(faces4Node, obj.PrimFace4s);
			}
		}

		this.DrawGameChunk(treeRoot);
	}

	public void SetAllFile(AllFile allFile) {
		this.AllFile = allFile;
		this.RedrawTree();
	}
}
