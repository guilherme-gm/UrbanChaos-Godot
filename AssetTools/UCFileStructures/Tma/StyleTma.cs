using Godot;

namespace AssetTools.UCFileStructures.Tma;

/**
 * Paths:
 * - server/textures/world%d/style.tma
 */
[Deserializer.DeserializeGenerator]
public partial class StyleTma : Resource
{
	public int SaveType { get; set; }

	/// <summary>Unused, I think</summary>
	[Deserializer.Condition(When = "value.SaveType >= 1 && value.SaveType < 4")]
	[Deserializer.Nested]
	public TextureInfoSection TextureInfoSection { get; set; }

	[Deserializer.Condition(When = "value.SaveType >= 1")]
	[Deserializer.Nested]
	public TextureXYSection TextureXYSection { get; set; }

	[Deserializer.Condition(When = "value.SaveType >= 1")]
	[Deserializer.Nested]
	public TextureStyleNameSection TextureStyleNameSection { get; set; }

	[Deserializer.Condition(When = "value.SaveType >= 2")]
	[Deserializer.Nested]
	public TextureFlagsSection TextureFlagsSection { get; set; }
}
