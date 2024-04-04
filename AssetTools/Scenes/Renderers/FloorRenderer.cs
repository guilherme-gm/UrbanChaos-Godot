using AssetTools.UCWorld.Maps;
using Godot;
using System.Collections.Generic;

namespace AssetTools.Scenes.Renderers;

[Tool]
public partial class FloorRenderer : Node3D
{
	private List<FloorFace> FloorFaces { get; set; }

	public void SetFloorFaces(List<FloorFace> floorFaces) {
		this.FloorFaces = floorFaces;
		this.Render();
	}

	private void DrawFacesWithTexture(int texturePage, List<FloorFace> floorFaces) {
		_ = texturePage;
		SurfaceTool st = new SurfaceTool();
		// st.SetMaterial

		st.Begin(Mesh.PrimitiveType.Triangles);

		int idx = 0;
		foreach (var face in floorFaces) {
			foreach (var vertex in face.Vertices) {
				st.SetUV(vertex.UV);
				st.SetNormal(Vector3.Up);
				st.AddVertex(vertex.Position / 256);
			}

			st.AddIndex(idx + 0);
			st.AddIndex(idx + 1);
			st.AddIndex(idx + 2);

			st.AddIndex(idx + 1);
			st.AddIndex(idx + 3);
			st.AddIndex(idx + 2);

			idx += 4;
		}

		this.AddChild(new MeshInstance3D() {
			Mesh = st.Commit(),
		});
	}

	private void Render() {
		foreach (var child in this.GetChildren()) {
			child.QueueFree();
		}

		if (this.FloorFaces == null) {
			return;
		}

		var textureGroups = new Dictionary<int, List<FloorFace>>();
		foreach (var face in this.FloorFaces) {
			var group = textureGroups.GetValueOrDefault(face.TexturePage);
			if (group == null) {
				group = [];
				textureGroups[face.TexturePage] = group;
			}

			group.Add(face);
		}

		foreach (var group in textureGroups) {
			this.DrawFacesWithTexture(group.Key, group.Value);
		}
	}
}
