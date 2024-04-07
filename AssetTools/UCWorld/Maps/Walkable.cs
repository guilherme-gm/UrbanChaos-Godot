using System.Collections.Generic;

namespace AssetTools.UCWorld.Maps;

public class Walkable
{
	public List<MapVertex[]> Quads { get; set; }
	public List<MapVertex[]> Triangles { get; set; }
}
