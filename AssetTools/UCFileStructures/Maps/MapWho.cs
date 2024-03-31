namespace AssetTools.UCFileStructures.Map;

/// <summary>
/// Describes Object in a LOW RES Map.
/// A LowRes Map is a 32x32 Map (while HighRes Map is 128x128)
/// </summary>
public struct Mapwho
{
	/// <summary>
	/// Index in Ob_Ob (MapObject Objects[])
	/// </summary>
	public int Index;

	/// <summary>
	/// How many instances of this object is present in this space
	/// </summary>
	public int Num;

	public static explicit operator Mapwho(ushort value) {
		return new Mapwho() {
			Index = value & ((1 << 11) - 1),
			Num = value >> 11,
		};
	}
}
