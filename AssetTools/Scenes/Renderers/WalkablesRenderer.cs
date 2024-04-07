using AssetTools.AssetManagers;
using AssetTools.UCWorld;
using AssetTools.UCWorld.Maps;
using Godot;
using System.Collections.Generic;

namespace AssetTools.Scenes.Renderers;

[Tool]
public partial class WalkablesRenderer : Node3D
{
	private string TextureClump { get; set; } = "";

	private List<Walkable> Walkables { get; set; }

	public void SetWalkables(string textureClump, List<Walkable> walkables) {
		this.TextureClump = textureClump;
		this.Walkables = walkables;
		this.Render();
	}

	private void DrawWithTexture(int texturePage, List<MapVertex[]> verticesList) {
		_ = texturePage;
		SurfaceTool st = new SurfaceTool();

		st.Begin(Mesh.PrimitiveType.Triangles);

		/**
		 * Original code disables culling for facets rendering.
		 * From a few tests, it seems like simply making the Indexes in a Clock-Wise order
		 * is enough to re-enable culling, but I chose to go the safe route.
		 */
		var material = TextureManager.Instance.LoadMaterial(this.TextureClump, -1);
		material.CullMode = BaseMaterial3D.CullModeEnum.Disabled;
		st.SetMaterial(material);

		int idx = 0;
		foreach (var vertices in verticesList) {
			foreach (var vertex in vertices) {
				st.SetUV(vertex.UV);
				st.SetNormal(Vector3.Back);
				st.AddVertex(vertex.Position / 256);
			}

			if (vertices.Length == 4) {
				st.AddIndex(idx + 0);
				st.AddIndex(idx + 1);
				st.AddIndex(idx + 2);

				st.AddIndex(idx + 3);
				st.AddIndex(idx + 2);
				st.AddIndex(idx + 1);
			} else {
				st.AddIndex(idx + 0);
				st.AddIndex(idx + 1);
				st.AddIndex(idx + 2);

				st.AddIndex(idx + 3);
				st.AddIndex(idx + 4);
				st.AddIndex(idx + 5);
			}

			idx += vertices.Length;
		}

		this.AddChild(new MeshInstance3D() {
			Mesh = st.Commit(),
		});
	}

	private void Render() {
		foreach (var child in this.GetChildren()) {
			child.QueueFree();
		}

		if (this.Walkables == null) {
			return;
		}

		var walkTextureGroups = new Dictionary<int, List<MapVertex[]>>();
		foreach (var facet in this.Walkables) {
			foreach (var quadVertices in facet.Quads) {
				var group = walkTextureGroups.GetValueOrDefault(quadVertices[0].TexturePage);
				if (group == null) {
					group = [];
					walkTextureGroups[quadVertices[0].TexturePage] = group;
				}

				group.Add(quadVertices);
			}

			foreach (var triVertices in facet.Triangles) {
				var group = walkTextureGroups.GetValueOrDefault(triVertices[0].TexturePage);
				if (group == null) {
					group = [];
					walkTextureGroups[triVertices[0].TexturePage] = group;
				}

				group.Add(triVertices);
			}
		}

		foreach (var group in walkTextureGroups) {
			this.DrawWithTexture(group.Key, group.Value);
		}
	}
}
