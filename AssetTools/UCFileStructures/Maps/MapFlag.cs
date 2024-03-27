namespace AssetTools.UCFileStructures.Maps;

public record MapFlag(ushort Value, string Name)
{
	public static MapFlag Shadow1 { get; } = new(1 << 0, "Shadow1");
	public static MapFlag Shadow2 { get; } = new(1 << 1, "Shadow2");
	public static MapFlag Shadow3 { get; } = new(1 << 2, "Shadow3");
	public static MapFlag Reflective { get; } = new(1 << 3, "Reflective");
	public static MapFlag Hidden { get; } = new(1 << 4, "Hidden");

	/// <summary>
	/// Lowers the floorsquare to create a curb.
	/// </summary>
	public static MapFlag SinkSquare { get; } = new(1 << 5, "SinkSquare");

	/// <summary>
	/// Transform the point on the lower level.
	/// </summary>
	public static MapFlag SinkPoint { get; } = new(1 << 6, "SinkPoint");

	/// <summary>
	/// Don't transform the point on the upper level.
	/// </summary>
	public static MapFlag NoUpper { get; } = new(1 << 7, "NoUpper");

	/// <summary>
	/// A square nobody is allowed onto
	/// </summary>
	public static MapFlag NoGo { get; } = new(1 << 8, "NoGo");
	public static MapFlag AnimTMap { get; } = new(1 << 9, "AnimTMap");
	public static MapFlag RoofExists { get; } = new(1 << 9, "RoofExists");

	/// <summary>
	/// These four bits identify groups of mapsquares
	/// </summary>
	public static MapFlag Zone1 { get; } = new(1 << 10, "Zone1");

	/// <summary>
	/// used by the AI system to gives zones.
	/// </summary>
	public static MapFlag Zone2 { get; } = new(1 << 11, "Zone2");
	public static MapFlag Zone3 { get; } = new(1 << 12, "Zone3");
	public static MapFlag Zone4 { get; } = new(1 << 13, "Zone4");
	public static MapFlag Wander { get; } = new(1 << 14, "Wander");
	public static MapFlag FlatRoof { get; } = new(1 << 14, "FlatRoof");
	public static MapFlag Water { get; } = new(1 << 15, "Water");

	public bool IsSet(int value) {
		return (value & this.Value) == this.Value;
	}
}
