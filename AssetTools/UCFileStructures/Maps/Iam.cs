using AssetTools.UCFileStructures.Maps.SuperMap;

namespace AssetTools.UCFileStructures.Maps;

[Deserializer.DeserializeGenerator]
public partial class Iam
{
	[Deserializer.Skip]
	public static readonly int MapHiSize = 128;

	[Deserializer.Skip]
	public static readonly int MapLoSize = 32;

	public int SaveType { get; set; }

	[Deserializer.Condition(When = $"value.{nameof(SaveType)} > 23")]
	public int OBSize { get; set; }

	/** The hi-res map. [x][z] */
	[Deserializer.FixedArray(Dimensions = [128, 128])]
	[Deserializer.Nested]
	public MapHi[][] HighResMap { get; set; }

	/** PSX (".pam") has some extra stuff here for rooftops (save_psx&&save_type>=26) */

	[Deserializer.Condition(When = $"value.{nameof(SaveType)} == 18")]
	[Deserializer.Nested]
	public MapThingPsxSection MapThingPsxSection { get; set; }

	[Deserializer.Condition(When = $"value.{nameof(SaveType)} > 18")]
	[Deserializer.Nested]
	public LoadGameThingSection LoadGameThingSection { get; set; }

	[Deserializer.Condition(When = $"value.{nameof(SaveType)} < 23")]
	[Deserializer.Nested]
	public MapObjectSection OldMapObjects { get; set; }

	[Deserializer.Nested(AdditionalParams = ["value.SaveType"])]
	public SuperMapSection SuperMap { get; set; }

	[Deserializer.Condition(When = $"value.{nameof(SaveType)} >= 20")]
	public int TextureSet { get; set; }

	[Deserializer.Condition(When = $"value.{nameof(SaveType)} >= 25")]
	[Deserializer.FixedArray(Dimensions = [2 * 200 * 5])]
	public byte[] PsxTexturesXY { get; set; }

	[Deserializer.Skip]
	public MapObjectSection MapObjects => this.SaveType < 23
		? this.OldMapObjects
		: this.SuperMap.SupermapMapObjects;
}
