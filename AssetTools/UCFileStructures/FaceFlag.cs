using AssetTools.Utils;

namespace AssetTools.UCFileStructures;

public record FaceFlag(uint Value, string Name) : IFlagsRecord
{
	public static FaceFlag Gourad { get; } = new(1 << 0, "Gourad");
	public static FaceFlag Textured { get; } = new(1 << 1, "Textured");
	public static FaceFlag Masked { get; } = new(1 << 2, "Masked");
	public static FaceFlag SemiTrans { get; } = new(1 << 3, "SemiTrans");
	public static FaceFlag Alpha { get; } = new(1 << 4, "Alpha");
	public static FaceFlag Tiled { get; } = new(1 << 5, "Tiled");
	public static FaceFlag DoubleSided { get; } = new(1 << 6, "DoubleSided");
	public static FaceFlag Walkable { get; } = new(1 << 7, "Walkable");

	private static readonly FaceFlag[] FlagList = [
		Gourad,
		Textured,
		Masked,
		SemiTrans,
		Alpha,
		Tiled,
		DoubleSided,
		Walkable,
	];

	public static Flags<FaceFlag> FromNumber(uint value) {
		return Flags<FaceFlag>.FromNumber(value, FlagList);
	}
}

