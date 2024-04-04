using AssetTools.Scenes.Renderers;
using Godot;

namespace AssetTools.UCWorld.Maps;

[Tool]
public partial class MapRenderer : Node3D
{
	[Export]
	private FloorRenderer FloorRenderer { get; set; }

	private string TextureClump { get; set; } = "";

	private UCMap Map { get; set; }

	public void SetMap(string textureClump, UCMap map) {
		this.Map = map;
		this.TextureClump = textureClump;
		this.FloorRenderer.SetFloorFaces(this.TextureClump, this.Map.FloorFaces);
	}
}
