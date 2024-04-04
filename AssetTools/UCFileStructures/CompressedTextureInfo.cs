using Godot;

namespace AssetTools.UCFileStructures;

/// <summary>
/// Describes a ushort value that contains:
/// - TexturePage (10 bits)
/// - Rotation (2 bits)
/// - Flip (2 bits)
/// - Size (2 bits)
/// </summary>
public partial class CompressedTextureInfo
{
	private static readonly Vector2[] Rotation0UV = [
		new Vector2(0, 0),
		new Vector2(1, 0),
		new Vector2(0, 1),
		new Vector2(1, 1),
	];

	private static readonly Vector2[] Rotation1UV = [
		new Vector2(1, 0),
		new Vector2(1, 1),
		new Vector2(0, 0),
		new Vector2(0, 1),
	];

	private static readonly Vector2[] Rotation2UV = [
		new Vector2(1, 1),
		new Vector2(0, 1),
		new Vector2(1, 0),
		new Vector2(0, 0),
	];

	private static readonly Vector2[] Rotation3UV = [
		new Vector2(0, 1),
		new Vector2(0, 0),
		new Vector2(1, 1),
		new Vector2(1, 0),
	];

	/// <summary>
	/// Decompressed into TexturePage / TextureRotation / TextureFlip / TextureSize
	/// up to 9th bit ( &(2^10-1) ) -- Texture Page -- texture & 0x3ff
	/// 10th and 11th bits -- Rotation -- (texture >> 0xa) & 0x3;
	/// 12th and 13th bits -- Flip -- (texture >> 0xc) & 0x3;
	/// 14th and 15th bits -- Size -- (texture >> 0xe) & 0x3;
	/// </summary>
	public ushort CompressedValue { get; set; }

	public int TexturePage { get; set; }

	public int TextureRotation { get; set; }

	public int TextureFlip { get; set; }

	public int TextureSize { get; set; }

	public Vector2[] UVs { get; set; }

	public static explicit operator CompressedTextureInfo(ushort value) {
		var result = new CompressedTextureInfo() {
			CompressedValue = value,
			TexturePage = value & 1023,
			TextureRotation = (value >> 10) & 3,
			TextureFlip = (value >> 12) & 3,
			TextureSize = (value >> 14) & 3,
		};

		switch (result.TextureRotation) {
			case 0: result.UVs = [.. Rotation0UV]; break;
			case 1: result.UVs = [.. Rotation1UV]; break;
			case 2: result.UVs = [.. Rotation2UV]; break;
			case 3: result.UVs = [.. Rotation3UV]; break;
			default:
				GD.PushError($"Could not create UV data for rotation {result.TextureRotation}");
				break;
		}

		return result;
	}
}
