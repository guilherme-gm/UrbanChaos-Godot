using AssetTools.Utils;

namespace AssetTools.UCFileStructures.Maps.SuperMap;

public record RoofFace4DrawFlags(uint Value, string Name) : IFlagsRecord
{
	/// <summary>Form a number from 0-7</summary>
	public static RoofFace4DrawFlags Shadow1 { get; } = new(1 << 0, "Shadow 1");

	public static RoofFace4DrawFlags Shadow2 { get; } = new(1 << 1, "Shadow 2");

	public static RoofFace4DrawFlags Shadow3 { get; } = new(1 << 2, "Shadow 3");

	public static RoofFace4DrawFlags SlideEdge0 { get; } = new(1 << 3, "Slide Edge 0");
	public static RoofFace4DrawFlags SlideEdge1 { get; } = new(1 << 4, "Slide Edge 1");
	public static RoofFace4DrawFlags SlideEdge2 { get; } = new(1 << 5, "Slide Edge 2");
	public static RoofFace4DrawFlags SlideEdge3 { get; } = new(1 << 6, "Slide Edge 3");
	public static RoofFace4DrawFlags NoDraw { get; } = new(1 << 7, "No Draw");

	private static readonly RoofFace4DrawFlags[] FlagList = [
		Shadow1,
		Shadow2,
		Shadow3,
		SlideEdge0,
		SlideEdge1,
		SlideEdge2,
		SlideEdge3,
		NoDraw,
	];

	public static Flags<RoofFace4DrawFlags> FromNumber(uint value) {
		return Flags<RoofFace4DrawFlags>.FromNumber(value, FlagList);
	}
}
