using AssetTools.Scenes.Renderers;
using Godot;

namespace AssetTools.UCWorld.Maps;

[Tool]
public partial class MapRenderer : Node3D
{
	[Export]
	private PolyListRenderer FloorRenderer { get; set; }

	[Export]
	private PolyListRenderer FacetRenderer { get; set; }

	[Export]
	private PolyListRenderer WalkableRenderer { get; set; }

	private string TextureClump { get; set; } = "";

	private UCMap Map { get; set; }

	public void SetMap(string textureClump, UCMap map) {
		this.Map = map;
		this.TextureClump = textureClump;
		this.FloorRenderer.SetPolyList(this.TextureClump, this.Map.FloorFaces);
		this.FacetRenderer.SetPolyList(this.TextureClump, this.Map.Facets);
		this.WalkableRenderer.SetPolyList(this.TextureClump, this.Map.Walkables);
	}
}
