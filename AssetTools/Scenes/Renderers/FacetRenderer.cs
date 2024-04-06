using AssetTools.AssetManagers;
using AssetTools.UCWorld;
using AssetTools.UCWorld.Maps;
using Godot;
using System.Collections.Generic;

namespace AssetTools.Scenes.Renderers;

[Tool]
public partial class FacetRenderer : Node3D
{
	private string TextureClump { get; set; } = "";

	private List<Facet> Facets { get; set; }

	public void SetFacets(string textureClump, List<Facet> facets) {
		this.TextureClump = textureClump;
		this.Facets = facets;
		this.Render();
	}

	private void DrawQuadsWithTexture(int texturePage, List<MapVertex[]> quads) {
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
		foreach (var quad in quads) {
			foreach (var vertex in quad) {
				st.SetUV(vertex.UV);
				st.SetNormal(Vector3.Back);
				st.AddVertex(vertex.Position / 256);
			}

			/**
			 *  3 - 2
			 *  | / |
			 *  1 - 0
			 */
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

		var textureGroups = new Dictionary<int, List<MapVertex[]>>();
		foreach (var facet in this.Facets) {
			foreach (var quadVertices in facet.Quads) {
				var group = textureGroups.GetValueOrDefault(quadVertices[0].TexturePage);
				if (group == null) {
					group = [];
					textureGroups[quadVertices[0].TexturePage] = group;
				}

				group.Add(quadVertices);
			}
		}

		foreach (var group in textureGroups) {
			this.DrawQuadsWithTexture(group.Key, group.Value);
		}
	}
}
