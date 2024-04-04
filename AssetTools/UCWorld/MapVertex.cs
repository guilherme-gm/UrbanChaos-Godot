using Godot;

namespace AssetTools.UCWorld;

public class MapVertex
{
	public Vector3 Position { get; set; }
	public Color Color { get; set; }
	public float Specular { get; set; }

	public Vector2 UV { get; set; }
}
