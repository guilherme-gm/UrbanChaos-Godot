using AssetTools.UCFileStructures.Maps;
using Godot;

namespace AssetTools.Structures;

// @TODO: This is probably wrong. But is the best storage I can think for now...
public class FloorStore
{
	public int Colour { get; set; }
	public float Alt { get; set; }
	public MapFlag[] Flags;   // not really needed

	public int TexturePage; // 309 runs "general steam" whatever this is. Maybe uses the remaining part of Texture

	public int X { get; set; }

	public int Z { get; set; }

	public Vector3[] Vertices { get; set; }
	public Vector2[] UVs { get; set; }

	public uint Specular { get; set; } = 0xff000000;

	public int Height { get; set; }

	public static FloorStore FromMapHi(MapHi mapHi, int index) {
		var x = index / 128;
		var z = index % 128;

		var alt = mapHi.Altitude;
		var size = 1f;
		var size2 = 1f; // originally, this is 256, but this is extremally HUGE

		return new FloorStore() {
			Colour = 0, // Converted from night data (cache_a_row)
			Alt = alt,
			Flags = mapHi.Flags,
			TexturePage = mapHi.Texture.TexturePage,
			X = x,
			Z = z,
			// @TODO: In original one, it checks 2 floors and calculates Y / ALT based on both.
			//        Also takes Kerbs into account
			Vertices = [
				new Vector3(x * size2, alt * size, z * size2),
				new Vector3((x + 1) * size2, alt * size, z * size2),
				new Vector3((x + 1) * size2, alt * size, (z + 1) * size2),
				new Vector3(x * size2, alt * size, (z + 1) * size2),
			],
			UVs = mapHi.Texture.UVs,
			Height = mapHi.Height,
		};
	}
}
