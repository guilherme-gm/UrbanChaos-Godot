using System;
using System.Collections.Generic;

namespace AssetTools.UCFileStructures.Maps.SuperMap;

public record BuildingType(ushort Id, string Name)
{
	public static BuildingType House { get; } = new(0, "house");
	public static BuildingType Warehouse { get; } = new(1, "warehouse");
	public static BuildingType Office { get; } = new(2, "office");
	public static BuildingType Apartement { get; } = new(3, "apartement");
	public static BuildingType CrateIn { get; } = new(4, "crate in");
	public static BuildingType CrateOut { get; } = new(5, "crate out");

	private static readonly List<BuildingType> BuildingTypes = [
		House,
		Warehouse,
		Office,
		Apartement,
		CrateIn,
		CrateOut,
	];

	public static explicit operator BuildingType(int value) {
		var index = BuildingTypes.FindIndex((v) => v.Id == value);
		if (index == -1) {
			throw new InvalidCastException($"Could not cast int ({value}) to BuildingType");
		}

		return BuildingTypes[index];
	}
}
