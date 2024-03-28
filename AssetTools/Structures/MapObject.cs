using Godot;

namespace AssetTools.Structures;

public class MapObject
{
	public Vector3 Position { get; set; }
	public int Prim { get; set; }
	public int Yaw { get; set; }
	public int Pitch { get; set; }
	public int Roll { get; set; }
	public int Index { get; set; }
	public int Crumple { get; set; }
	public byte Flags { get; set; }
	public byte InsideIndex { get; set; }
}
