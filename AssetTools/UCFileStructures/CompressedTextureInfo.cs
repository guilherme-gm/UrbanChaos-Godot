using Godot;

namespace AssetTools.UCFileStructures;

/// <summary>
/// Describes a ushort value that contains:
/// - TexturePage (10 bits)
/// - Rotation (2 bits)
/// - Flip (2 bits)
/// - Size (2 bits)
/// </summary>
[Deserializer.DeserializeGenerator]
public partial class CompressedTextureInfo
{
	[Deserializer.Skip]
	private static readonly Vector2[] Rotation0UV = [
		new Vector2(0, 0),
		new Vector2(1, 0),
		new Vector2(1, 1),
		new Vector2(0, 1),
	];

	[Deserializer.Skip]
	private static readonly Vector2[] Rotation1UV = [
		new Vector2(1, 0),
		new Vector2(1, 1),
		new Vector2(0, 1),
		new Vector2(0, 0),
	];

	[Deserializer.Skip]
	private static readonly Vector2[] Rotation2UV = [
		new Vector2(1, 1),
		new Vector2(0, 1),
		new Vector2(0, 0),
		new Vector2(1, 0),
	];

	[Deserializer.Skip]
	private static readonly Vector2[] Rotation3UV = [
		new Vector2(0, 1),
		new Vector2(0, 0),
		new Vector2(1, 0),
		new Vector2(1, 1),
	];

	/// <summary>
	/// Decompressed into TexturePage / TextureRotation / TextureFlip / TextureSize
	/// up to 9th bit ( &(2^10-1) ) -- Texture Page -- texture & 0x3ff
	/// 10th and 11th bits -- Rotation -- (texture >> 0xa) & 0x3;
	/// 12th and 13th bits -- Flip -- (texture >> 0xc) & 0x3;
	/// 14th and 15th bits -- Size -- (texture >> 0xe) & 0x3;
	/// </summary>
	private ushort CompressedTexture { get; set; }

	[Deserializer.Skip]
	public int TexturePage { get; set; }

	[Deserializer.Skip]
	public int TextureRotation { get; set; }

	[Deserializer.Skip]
	public int TextureFlip { get; set; }

	[Deserializer.Skip]
	public int TextureSize { get; set; }

	[Deserializer.Skip]
	public Vector2[] UVs { get; set; }

	partial void PostDeserialize() {
		this.TexturePage = this.CompressedTexture & 1023;
		this.TextureRotation = (this.CompressedTexture >> 10) & 3;
		this.TextureFlip = (this.CompressedTexture >> 12) & 3;
		this.TextureSize = (this.CompressedTexture >> 14) & 3;

		switch (this.TextureRotation) {
			case 0: this.UVs = Rotation0UV; break;
			case 1: this.UVs = [.. Rotation1UV]; break;
			case 2: this.UVs = [.. Rotation2UV]; break;
			case 3: this.UVs = [.. Rotation3UV]; break;
			default:
				GD.PushError($"Could not create UV data for rotation {this.TextureRotation}");
				break;
		}
	}
}
