using AssetTools.UCFileStructures.Maps.SuperMap;
using AssetTools.UCWorld.Maps;
using AssetTools.UCWorld.Utils;
using Godot;
using System;

namespace AssetTools.UCWorld.Textures;

// @TODO: Is there a better place to move that?

public static class TextureUtils
{
	private const int TEXTURE_PIECE_LEFT = 0;
	private const int TEXTURE_PIECE_RIGHT = 2;
	private const int TEXTURE_PIECE_MIDDLE = 1;
	private const int TEXTURE_PIECE_MIDDLE1 = 3;
	private const int TEXTURE_PIECE_MIDDLE2 = 4;

	private static readonly int[] TextureChoices = [
		TEXTURE_PIECE_MIDDLE,
		TEXTURE_PIECE_MIDDLE,
		TEXTURE_PIECE_MIDDLE1,
		TEXTURE_PIECE_MIDDLE2,
	];

	public static int TextureQuad(FacetTextureRNG textureRNG, UCMap map, MapVertex[] polyPoints, int textureStyle, int pos, int count, int flipx = 0) {
		var iam = map.Iam.Data;
		var textureSet = map.TextureSet;
		var rand = textureRNG.Next() & 3;

		int texturePiece;
		if (pos == 0) {
			texturePiece = flipx != 0 ? TEXTURE_PIECE_LEFT : TEXTURE_PIECE_RIGHT;
		} else if (pos == count - 2) {
			texturePiece = flipx != 0 ? TEXTURE_PIECE_RIGHT : TEXTURE_PIECE_LEFT;
		} else {
			texturePiece = TextureChoices[rand];
		}

		int page = 0;
		TextureFlip flip = TextureFlip.None;
		if (textureStyle < 0) {
			var storey = iam.SuperMap.DStoreys[-textureStyle];
			var index = storey.Index;

			if (storey.Count == 0) {
				GD.PushError($"Storey count is 0. Should be > 0");
			}

			if (pos < storey.Count) {
				page = iam.SuperMap.PaintMem[index + pos];
				if ((page & 128) != 0) {
					flip = TextureFlip.FlipX;
					page &= 127;
				}
			} else {
				textureStyle = storey.Style;
			}

			if ((page & 127) == 0) {
				textureStyle = storey.Style;
			}
		}

		if (textureStyle >= 0) {
			if (textureStyle == 0) {
				textureStyle = 1;
			}

			if (textureStyle > textureSet.DxTextureXYs.Length) {
				// @FIXME: What is happening? this is not in the original code. but their array is bigger...
				GD.PushWarning($"Out of range");
				page = 0;
				flip = TextureFlip.None;
			} else {
				page = textureSet.DxTextureXYs[textureStyle][texturePiece].Page;
				flip = textureSet.DxTextureXYs[textureStyle][texturePiece].Flip;
			}
		}

		switch (flip.Id) {
			case 0: // TextureFlip.None:
				polyPoints[0].UV = new Vector2(0, 0);
				polyPoints[1].UV = new Vector2(1, 0);
				polyPoints[2].UV = new Vector2(0, 1);
				polyPoints[3].UV = new Vector2(1, 1);
				break;

			case 1: // flip x
				polyPoints[0].UV = new Vector2(1, 0);
				polyPoints[1].UV = new Vector2(0, 0);
				polyPoints[2].UV = new Vector2(1, 1);
				polyPoints[3].UV = new Vector2(0, 1);
				break;

			case 2: // flip y
				polyPoints[0].UV = new Vector2(0, 0);
				polyPoints[1].UV = new Vector2(1, 0);
				polyPoints[2].UV = new Vector2(0, 0);
				polyPoints[3].UV = new Vector2(1, 0);
				break;

			case 3: // flip x + y
				polyPoints[0].UV = new Vector2(1, 1);
				polyPoints[1].UV = new Vector2(0, 1);
				polyPoints[2].UV = new Vector2(1, 0);
				polyPoints[3].UV = new Vector2(0, 0);
				break;

			default:
				throw new Exception($"Invalid flip {flip}");
		}

		return page;
	}
}
