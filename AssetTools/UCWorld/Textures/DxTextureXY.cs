using AssetTools.UCFileStructures.Maps.SuperMap;
using AssetTools.UCFileStructures.Tma;

namespace AssetTools.UCWorld.Textures;

public class DxTextureXY
{
	private const int TEXTURE_NORM_SIZE = 32;

	private const int TEXTURE_NORM_SQUARES = 8;

	public int Page { get; set; }
	public TextureFlip Flip { get; set; }

	public static DxTextureXY FromTextureXY(TextureXY textureXY) {
		var base_u = textureXY.Tx * 32;
		var base_v = textureXY.Ty * 32;

		var av_u = base_u / TEXTURE_NORM_SIZE;
		var av_v = base_v / TEXTURE_NORM_SIZE;

		var page = av_u + (av_v * TEXTURE_NORM_SQUARES) + (textureXY.Page * TEXTURE_NORM_SQUARES * TEXTURE_NORM_SQUARES);

		return new DxTextureXY() {
			Page = page,
			Flip = textureXY.Flip,
		};
	}
}
