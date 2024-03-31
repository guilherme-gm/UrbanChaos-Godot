using System;
using System.Collections.Generic;

namespace AssetTools.UCFileStructures.Maps.SuperMap;

public record FacetType(ushort Id, string Name)
{
	public static FacetType None { get; } = new(0, "None"); // Not part of original code, but some places sets it to 0.
	public static FacetType Normal { get; } = new(1, "Normal");
	public static FacetType Roof { get; } = new(2, "Roof");
	public static FacetType Wall { get; } = new(3, "Wall");
	public static FacetType RoofQuad { get; } = new(4, "RoofQuad");
	public static FacetType FloorPoints { get; } = new(5, "FloorPoints");
	public static FacetType FireEscape { get; } = new(6, "FireEscape");
	public static FacetType Staircase { get; } = new(7, "Staircase");
	public static FacetType Skylight { get; } = new(8, "Skylight");
	public static FacetType Cable { get; } = new(9, "Cable");
	public static FacetType Fence { get; } = new(10, "Fence");
	public static FacetType FenceBrick { get; } = new(11, "FenceBrick");
	public static FacetType Ladder { get; } = new(12, "Ladder");
	public static FacetType FenceFlat { get; } = new(13, "FenceFlat");
	public static FacetType Trench { get; } = new(14, "Trench");
	public static FacetType JustCollision { get; } = new(15, "JustCollision");
	public static FacetType Partition { get; } = new(16, "Partition");
	public static FacetType Inside { get; } = new(17, "Inside");
	public static FacetType Door { get; } = new(18, "Door");
	public static FacetType InsideDoor { get; } = new(19, "InsideDoor");
	public static FacetType Oinside { get; } = new(20, "Oinside");
	public static FacetType OutsideDoor { get; } = new(21, "OutsideDoor");

	public static FacetType NormalFoundation { get; } = new(100, "NormalFoundationn");

	public static FacetType NotReallyAStoreyTypeButAValueToPutInThePrimTypeFieldOfColvectsGeneratedByInsideBuildings { get; } = new(254, "NotReallyAStoreyTypeButAValueToPutInThePrimTypeFieldOfColvectsGeneratedByInsideBuildings");
	public static FacetType NotReallyAStoreyTypeAgainThisIsTheValuePutIntoPrimtypeByTheSewers { get; } = new(255, "NotReallyAStoreyTypeAgainThisIsTheValuePutIntoPrimtypeByTheSewers");

	private static readonly List<FacetType> FacetTypes = [
		None,
		Normal,
		Roof,
		Wall,
		RoofQuad,
		FloorPoints,
		FireEscape,
		Staircase,
		Skylight,
		Cable,
		Fence,
		FenceBrick,
		Ladder,
		FenceFlat,
		Trench,
		JustCollision,
		Partition,
		Inside,
		Door,
		InsideDoor,
		Oinside,
		OutsideDoor,
		NormalFoundation,
		NotReallyAStoreyTypeButAValueToPutInThePrimTypeFieldOfColvectsGeneratedByInsideBuildings,
		NotReallyAStoreyTypeAgainThisIsTheValuePutIntoPrimtypeByTheSewers,
	];

	public override string ToString() {
		return $"{this.Name} ({this.Id})";
	}

	public static explicit operator FacetType(int value) {
		var index = FacetTypes.FindIndex((v) => v.Id == value);
		if (index == -1) {
			throw new InvalidCastException($"Could not cast int ({value}) to FacetType");
		}

		return FacetTypes[index];
	}
}
