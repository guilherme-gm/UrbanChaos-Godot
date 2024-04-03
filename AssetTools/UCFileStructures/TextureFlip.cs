using System;
using System.Collections.Generic;

namespace AssetTools.UCFileStructures.Maps.SuperMap;

public record TextureFlip(ushort Id, string Name)
{
	public static TextureFlip None { get; } = new(0, "None");
	public static TextureFlip FlipX { get; } = new(1, "FlipX");
	public static TextureFlip FlipY { get; } = new(2, "FlipY");
	public static TextureFlip FlipXY { get; } = new(3, "FlipXY");

	private static readonly List<TextureFlip> FlipTypes = [
		None,
		FlipX,
		FlipY,
		FlipXY,
	];

	public static explicit operator TextureFlip(int value) {
		var index = FlipTypes.FindIndex((v) => v.Id == value);
		if (index == -1) {
			throw new InvalidCastException($"Could not cast int ({value}) to TextureFlip");
		}

		return FlipTypes[index];
	}
}
