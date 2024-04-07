using AssetTools.AssetManagers;
using AssetTools.UCWorld.Poly;
using Godot;
using System.Collections.Generic;

namespace AssetTools.Scenes.Renderers;

[Tool]
public partial class FacetRenderer : Node3D
{
	private string TextureClump { get; set; } = "";

	private List<IPoly> Facets { get; set; }

	public void SetFacets(string textureClump, List<IPoly> facets) {
		this.TextureClump = textureClump;
		this.Facets = facets;
		this.Render();
	}

	private void DrawPolysWithTexture(int texturePage, List<IPoly> polys) {
		_ = texturePage;
		SurfaceTool st = new SurfaceTool();

		st.Begin(Mesh.PrimitiveType.Triangles);

		/**
		 * Original code disables culling for facets rendering.
		 * From a few tests, it seems like simply making the Indexes in a Clock-Wise order
		 * is enough to re-enable culling, but I chose to go the safe route.
		 */
		var material = TextureManager.Instance.LoadMaterial(this.TextureClump, texturePage);
		material.CullMode = BaseMaterial3D.CullModeEnum.Disabled;
		st.SetMaterial(material);

		int idx = 0;
		foreach (var poly in polys) {
			foreach (var vertex in poly.GetVertices()) {
				st.SetUV(vertex.UV);
				st.SetNormal(Vector3.Back);
				st.AddVertex(vertex.Position / 256);
			}

			foreach (var index in poly.GetIndices()) {
				st.AddIndex(idx + index);
			}
			idx += poly.GetVertices().Length;

			st.AddIndex(idx + 0);
			st.AddIndex(idx + 2);
			st.AddIndex(idx + 1);

			st.AddIndex(idx + 1);
			st.AddIndex(idx + 2);
			st.AddIndex(idx + 3);

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

		if (this.Facets == null) {
			return;
		}

		var textureGroups = new Dictionary<int, List<IPoly>>();
		foreach (var poly in this.Facets) {
			var group = textureGroups.GetValueOrDefault(poly.GetTexturePage());
			if (group == null) {
				group = [];
				textureGroups[poly.GetTexturePage()] = group;
			}

			group.Add(poly);
		}

		foreach (var group in textureGroups) {
			this.DrawPolysWithTexture(group.Key, group.Value);
		}
	}
}
