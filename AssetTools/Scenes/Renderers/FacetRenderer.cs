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

	private void DrawFacetsWithTexture(int texturePage, List<Facet> facets) {
		_ = texturePage;
		SurfaceTool st = new SurfaceTool();

		st.Begin(Mesh.PrimitiveType.Triangles);
		// st.SetMaterial(TextureManager.Instance.LoadMaterial(this.TextureClump, texturePage));

		int idx = 0;
		GD.Print(facets.Count);
		foreach (var facet in facets) {
			for (int row = 0; row < facet.Vertices.Length - 1; row++) {
				for (int col = 0; col < facet.Vertices[row].Length - 1; col++) {
					var quad = new MapVertex[] {
						facet.Vertices[row][col + 1],
						facet.Vertices[row][col],
						facet.Vertices[row + 1][col + 1],
						facet.Vertices[row + 1][col]
					};

					foreach (var vertex in quad) {
						// st.SetUV(vertex.UV);
						st.SetNormal(Vector3.Up);
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
			}
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

		// @TODO: Group by texture
		this.DrawFacetsWithTexture(0, this.Facets);
	}
}
