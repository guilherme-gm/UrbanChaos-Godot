using Godot;

namespace AssetTools.UCWorld;

public class MapVertex
{
	public Vector3 Position { get; set; }
	public Color Color { get; set; }
	public float Specular { get; set; }

	public int TexturePage { get; set; } = -1;

	public Vector2 UV { get; set; }

	public MapVertex Clone() {
		return new MapVertex() {
			Position = new Vector3(this.Position.X, this.Position.Y, this.Position.Z),
			Color = new Color(this.Color, this.Color.A),
			Specular = this.Specular,
			TexturePage = this.TexturePage,
			UV = new Vector2(this.UV.X, this.UV.Y),
		};
	}
}
