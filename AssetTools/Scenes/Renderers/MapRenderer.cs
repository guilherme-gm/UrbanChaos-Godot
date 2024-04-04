using AssetTools.Scenes.Renderers;
using Godot;

namespace AssetTools.UCWorld.Maps;

[Tool]
public partial class MapRenderer : Node3D
{
	[Export]
	private FloorRenderer FloorRenderer { get; set; }

	private UCMap Map { get; set; }

	public void SetMap(UCMap map) {
		this.Map = map;
		this.FloorRenderer.SetFloorFaces(this.Map.FloorFaces);
	}
}
