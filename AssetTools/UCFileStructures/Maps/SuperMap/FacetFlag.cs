using AssetTools.Utils;

namespace AssetTools.UCFileStructures.Maps.SuperMap;

public record FacetFlag(uint Value, string Name) : IFlagsRecord
{
	/// <summary>facet is duplicate so mark invisible</summary>
	public static FacetFlag Invisible { get; } = new(1 << 0, "INVISIBLE");

	public static FacetFlag Inside { get; } = new(1 << 3, "INSIDE");

	/// <summary>Lit with a dynamic light.</summary>
	public static FacetFlag Dlit { get; } = new(1 << 4, "DLIT");

	/// <summary>For fake fences that are normal walls</summary>
	public static FacetFlag HugFloor { get; } = new(1 << 5, "HUG_FLOOR");

	/// <summary>For fences...</summary>
	public static FacetFlag Electrified { get; } = new(1 << 6, "ELECTRIFIED");

	public static FacetFlag TwoSided { get; } = new(1 << 7, "2SIDED");

	public static FacetFlag Unclimbable { get; } = new(1 << 8, "UNCLIMBABLE");

	public static FacetFlag Onbuilding { get; } = new(1 << 9, "ONBUILDING");

	public static FacetFlag BarbTop { get; } = new(1 << 10, "BARB_TOP");

	public static FacetFlag Seethrough { get; } = new(1 << 11, "SEETHROUGH");

	/// <summary>For OUTSIDE_DOOR facets...</summary>
	public static FacetFlag Open { get; } = new(1 << 12, "OPEN");

	/// <summary>Some OUTSIDE_DOOR facets open only by 90 degrees...</summary>
	public static FacetFlag NinetyDegree { get; } = new(1 << 13, "90DEGREE");

	public static FacetFlag TwoTextured { get; } = new(1 << 14, "2TEXTURED");

	public static FacetFlag FenceCut { get; } = new(1 << 15, "FENCE_CUT");

	private static readonly FacetFlag[] FlagList = [
		Invisible,
		Inside,
		Dlit,
		HugFloor,
		Electrified,
		TwoSided,
		Unclimbable,
		Onbuilding,
		BarbTop,
		Seethrough,
		Open,
		NinetyDegree,
		TwoTextured,
		FenceCut,
	];

	public static Flags<FacetFlag> FromNumber(uint value) {
		return Flags<FacetFlag>.FromNumber(value, FlagList);
	}
}
