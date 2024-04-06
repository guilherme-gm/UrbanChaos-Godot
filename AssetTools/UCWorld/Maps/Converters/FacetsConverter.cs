using AssetTools.UCFileStructures.Maps;
using AssetTools.UCFileStructures.Maps.SuperMap;
using AssetTools.UCWorld.Textures;
using AssetTools.UCWorld.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetTools.UCWorld.Maps.Converters;

public class FacetsConverter
{
	private class LoMapWhoCell
	{
		public List<DFacet> Facet = [];

		public LoMapWhoCell() {
			this.Facet = [];
		}
	}

	private readonly UCMap UCMap;

	private FacetTextureRNG TextureRNG;

	private Iam Iam => this.UCMap.Iam.Data;

	private TextureStyle TextureSet => this.UCMap.TextureSet;

	private readonly LoMapWhoCell[][] LoMapWho;

	private const int TEXTURE_PIECE_LEFT = 0;
	private const int TEXTURE_PIECE_RIGHT = 2;
	private const int TEXTURE_PIECE_MIDDLE = 1;
	private const int TEXTURE_PIECE_MIDDLE1 = 3;
	private const int TEXTURE_PIECE_MIDDLE2 = 4;

	private readonly int[] TextureChoices = [
		TEXTURE_PIECE_MIDDLE,
		TEXTURE_PIECE_MIDDLE,
		TEXTURE_PIECE_MIDDLE1,
		TEXTURE_PIECE_MIDDLE2,
	];

	public FacetsConverter(UCMap ucMap) {
		this.UCMap = ucMap;
		this.LoMapWho = new LoMapWhoCell[Iam.MapLoSize][];
		for (int i = 0; i < Iam.MapLoSize; i++) {
			this.LoMapWho[i] = new LoMapWhoCell[Iam.MapLoSize];
			for (int j = 0; j < Iam.MapLoSize; j++) {
				this.LoMapWho[i][j] = new LoMapWhoCell();
			}
		}
	}

	private static bool IsWithin(int val, int min, int max) {
		return val >= min && val <= max;
	}

	private void LinkFacetToMapWho(int mx, int mz, DFacet facet) {
		if (mx < 0 || mx > Iam.MapLoSize || mz < 0 || mz > Iam.MapLoSize) {
			GD.PrintErr($"LinkFacetToMapWho: out of bounds (MX: {mx} ; MZ: {mz})");
			return;
		}

		this.LoMapWho[mx][mz].Facet.Add(facet);
	}

	private void AddFacetToMap(DFacet facet) {
		int startX = facet.X[0].ToMap().Value;
		int endX = facet.X[1].ToMap().Value;
		int startZ = facet.Z[0].ToMap().Value;
		int endZ = facet.Z[1].ToMap().Value;

		int xDiff = endX - startX;
		int zDiff = endZ - startZ;

		if (xDiff == 0 && zDiff == 0)
			return;

		if (facet.FacetType == FacetType.Ladder) {
			// @TODO: calc_ladder_ends(&x1, &z1, &x2, &z2);
		}

		xDiff = endX - startX;
		zDiff = endZ - startZ;

		int absoluteXDiff = Mathf.Abs(xDiff);
		int absoluteZDiff = Mathf.Abs(zDiff);

		int startMapLoX = facet.X[0].ToLowRes().Value;
		int startMapLoZ = facet.Z[0].ToLowRes().Value;
		int endMapLoX = facet.X[1].ToLowRes().Value;
		int endMapLoZ = facet.Z[1].ToLowRes().Value;

		const int PAP_SHIFT_LO = 10;

		// Since x1 and z1 are Facet's X/Z shifted in 8 bits, this is basically the last 2 bits of the original value
		// converted to Map coordinates (maybe something like the "decimal place"? / "fraction")
		int xfrac = startX & ((1 << PAP_SHIFT_LO) - 1);
		int zfrac = startZ & ((1 << PAP_SHIFT_LO) - 1);

		if (absoluteXDiff > absoluteZDiff) { // X Span > Z Span
			int frac = (zDiff << PAP_SHIFT_LO) / xDiff;

			int z;
			if (xDiff > 0) {
				z = startZ;
				z -= (frac * xfrac) >> PAP_SHIFT_LO;
			} else {
				z = startZ;
				z += (frac * ((1 << PAP_SHIFT_LO) - xfrac)) >> PAP_SHIFT_LO;
			}

			while (true) {
				if (IsWithin(startMapLoX, 0, Iam.MapLoSize - 1)
					&& IsWithin(startMapLoZ, 0, Iam.MapLoSize - 1)
				) {
					this.LinkFacetToMapWho(startMapLoX, startMapLoZ, facet);
				}

				if (startMapLoX == endMapLoX &&
					startMapLoZ == endMapLoZ
				) {
					return;
				}

				//
				// Step in z.
				//

				if (xDiff > 0) {
					z += frac;
				} else {
					z -= frac;
				}

				if ((z >> PAP_SHIFT_LO) != startMapLoZ) {
					//
					// Step up/down in z through another mapsquare.
					//

					startMapLoZ = z >> PAP_SHIFT_LO;

					if (IsWithin(startMapLoX, 0, Iam.MapLoSize - 1) &&
						IsWithin(startMapLoZ, 0, Iam.MapLoSize - 1)
					) {
						this.LinkFacetToMapWho(startMapLoX, startMapLoZ, facet);
					}
				}

				//
				// Step in x.
				//

				if (xDiff > 0) {
					startMapLoX += 1;
					if (startMapLoX > endMapLoX) {
						return;
					}
				} else {
					startMapLoX -= 1;
					if (startMapLoX < endMapLoX) {
						return;
					}
				}
			}
		} else {
			int frac = (xDiff << PAP_SHIFT_LO) / zDiff;

			int x;
			if (zDiff > 0) {
				x = startX;
				x -= (frac * zfrac) >> PAP_SHIFT_LO;
			} else {
				x = startX;
				x += (frac * ((1 << PAP_SHIFT_LO) - zfrac)) >> PAP_SHIFT_LO;
			}

			while (true) {
				if (IsWithin(startMapLoX, 0, Iam.MapLoSize - 1) &&
					IsWithin(startMapLoZ, 0, Iam.MapLoSize - 1)
				) {
					this.LinkFacetToMapWho(startMapLoX, startMapLoZ, facet);
				}

				if (startMapLoX == endMapLoX &&
					startMapLoZ == endMapLoZ) {
					return;
				}

				//
				// Step in x.
				//

				if (zDiff > 0) {
					x += frac;
				} else {
					x -= frac;
				}

				if ((x >> PAP_SHIFT_LO) != startMapLoX) {
					//
					// Step up/down in z through another mapsquare.
					//

					startMapLoX = x >> PAP_SHIFT_LO;

					if (IsWithin(startMapLoX, 0, Iam.MapLoSize - 1) &&
						IsWithin(startMapLoZ, 0, Iam.MapLoSize - 1)
					) {
						this.LinkFacetToMapWho(startMapLoX, startMapLoZ, facet);
					}
				}

				//
				// Step in z.
				//

				if (zDiff > 0) {
					startMapLoZ += 1;

					if (startMapLoZ > endMapLoZ) {
						return;
					}
				} else {
					startMapLoZ -= 1;

					if (startMapLoZ < endMapLoZ) {
						return;
					}
				}
			}
		}
	}

	private void LoadFacetsFromBuildings() {
		foreach (var building in this.Iam.SuperMap.DBuildings) {
			for (int i = building.StartFacet; i < building.EndFacet; i++) {
				var facet = this.Iam.SuperMap.DFacets[i];
				// From original docs:
				// CRATE_INSIDEs facets must be added because it makes the floor of Poshetas disappear
				// (What is Poshetas?)
				if (facet.FacetFlags.IsSet(FacetFlag.Invisible)
					&& building.Type != BuildingType.CrateIn
				) {
					// Ignore this facet
					continue;
				}

				if (facet.FacetType == FacetType.Inside
					|| facet.FacetType == FacetType.Oinside
					|| facet.FacetType == FacetType.InsideDoor
				) {
					continue;
				}

				this.AddFacetToMap(facet);
			}
		}
	}

	private static bool IsRareFacet(DFacet facet) {
		if (facet.FacetType != FacetType.Normal) {
			return true;
		}

		// @TODO: What is INDOORS_INDEX ??
		// if (INDOORS_INDEX) {
		// 	return true;
		// }

		if (facet.FacetFlags.IsAnySet([FacetFlag.BarbTop, FacetFlag.TwoSided, FacetFlag.Inside])) {
			return true;
		}

		return false;
	}

	private static int CalcDirection(DFacet facet) {
		int direction;
		if (facet.Z[0] == facet.Z[1]) {
			if (facet.X[0].Value < facet.X[1].Value) {
				direction = 0;
			} else {
				direction = 2;
			}
		} else {
			if (facet.Z[0].Value > facet.Z[1].Value) {
				direction = 3;
			} else {
				direction = 1;
			}
		}

		return direction;
	}

	private int TextureQuad(MapVertex[] polyPoints, int textureStyle, int pos, int count, int flipx = 0) {
		var rand = this.TextureRNG.Next() & 3;
		int random = 0;

		int texturePiece;
		if (pos == 0) {
			texturePiece = flipx != 0 ? TEXTURE_PIECE_LEFT : TEXTURE_PIECE_RIGHT;
		} else if (pos == count - 2) {
			texturePiece = flipx != 0 ? TEXTURE_PIECE_RIGHT : TEXTURE_PIECE_LEFT;
		} else {
			texturePiece = this.TextureChoices[rand];
			if (rand > 1) {
				random = 1;
			}
		}

		_ = random; // @TODO: Use it or delete it.

		int page = 0;
		TextureFlip flip = TextureFlip.None;
		if (textureStyle < 0) {
			GD.PushWarning("@TODO: Implement textureStyle < 0 for facet");
			textureStyle = 0;
		}

		if (textureStyle >= 0) { // this can become a else later.
			if (textureStyle == 0) {
				textureStyle = 1;
			}

			if (textureStyle > this.UCMap.TextureSet.DxTextureXYs.Length) {
				// @FIXME: What is happening? this is not in the original code. but their array is bigger...
				GD.PushWarning($"Out of range");
				page = 0;
				flip = TextureFlip.None;
			} else {
				page = this.TextureSet.DxTextureXYs[textureStyle][texturePiece].Page;
				flip = this.TextureSet.DxTextureXYs[textureStyle][texturePiece].Flip;
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
				polyPoints[1].UV = new Vector2(1, 1);
				polyPoints[2].UV = new Vector2(0, 0);
				polyPoints[3].UV = new Vector2(0, 1);
				break;

			case 2: // flip y
				polyPoints[0].UV = new Vector2(1, 1);
				polyPoints[1].UV = new Vector2(0, 1);
				polyPoints[2].UV = new Vector2(1, 0);
				polyPoints[3].UV = new Vector2(0, 0);
				break;

			case 3: // flip x + y
				polyPoints[0].UV = new Vector2(0, 1);
				polyPoints[1].UV = new Vector2(0, 0);
				polyPoints[2].UV = new Vector2(1, 1);
				polyPoints[3].UV = new Vector2(1, 0);
				break;

			default:
				throw new Exception($"Invalid flip {flip}");
		}

		return page;
	}

	// SUPERFACET_create_calls -- originally creates "draw calls" with direction and texture
	private void SetupSuperfacetTextures(DFacet facet, int count, int height) {
		int styleIndex = facet.StyleIndex;
		if (facet.FacetFlags.IsSet(FacetFlag.TwoTextured)) {
			styleIndex--;
		}

		int styleIndexStep = 1;
		if (!facet.FacetFlags.IsSet(FacetFlag.HugFloor) && facet.FacetFlags.IsAnySet([FacetFlag.TwoSided, FacetFlag.TwoSided])) {
			styleIndexStep = 2;
		}

		this.TextureRNG = new FacetTextureRNG((uint)facet.X[0].Value, (uint)facet.Y[0], (uint)facet.Z[0].Value);

		// @FIXME: This is garbage. we can probably remove, but I am keeping until I finish converting everything.
		var __polyPoints = new MapVertex[] { new MapVertex(), new MapVertex(), new MapVertex(), new MapVertex() };

		int hf = 0;
		while (height >= 0) {
			if (hf != 0) {
				for (int i = 0; i < count - 1; i++) {
					int page = this.TextureQuad(__polyPoints, this.Iam.SuperMap.DStyles[styleIndex - 1], i, count);

					foreach (var pp in __polyPoints) {
						pp.TexturePage = page;
					}
				}

				_ = this.TextureRNG.Next();
			}

			height -= 4;
			hf += 1;
			styleIndex += styleIndexStep;
		}
	}

	private MapVertex[][] CreateSuperFacetPoints(
		float mapStartX,
		float mapStartY,
		float mapStartZ,
		float mapXStep,
		float mapZStep,
		float blockHeight,
		int height,
		Color col,
		int foundation,
		int horizontalPointCount,
		bool hasHug
	) {
		_ = col;

		List<List<MapVertex>> points = [];
		while (height >= 0) {
			float x = mapStartX;
			float y = mapStartY;
			float z = mapStartZ;

			int highX = (int)mapStartX / 256;
			int highZ = (int)mapStartZ / 256;

			List<MapVertex> row = [];
			points.Add(row);

			for (int c0 = horizontalPointCount; c0 > 0; c0--) {
				MapVertex pp = new MapVertex();

				float ty;
				if (hasHug) {
					ty = this.Iam.HighResMap[highX][highZ].Alt.ToMap().Value;
					ty += y;
				} else if (foundation != 2) {
					ty = y;
				} else {
					ty = this.Iam.HighResMap[highX][highZ].Alt.ToMap().Value;

					// @TODO:
					// FacetDiffY[POLY_buffer_upto - 1] = ((y - ty) * (1.0f / 256.0f)) + 1.0f;
				}

				pp.Position = new Vector3(x, ty, z);
				row.Add(pp);

				//
				// The index into the dfcache array for this facet.
				//

				// pp->user = col - SUPERFACET_colour_base;

				// NIGHT_get_d3d_colour(*col, &pp->colour, &pp->specular);

				x += mapXStep;
				z += mapZStep;
				// col += 1;
			}

			mapStartY += blockHeight;
			height -= 4;
			foundation -= 1;
		}

		return [.. points.Select<List<MapVertex>, MapVertex[]>((List<MapVertex> p) => [.. p])];
	}

	private List<MapVertex[]> FillFacetPoints(MapVertex[][] points, int count, uint baseRow, int foundation, int styleIndex, float blockHeight) {
		float vheight = blockHeight * (1.0f / 256.0f);

		uint row1 = baseRow;
		uint row2 = baseRow + 1;

		var quadList = new List<MapVertex[]>();

		for (int i = 0; i < points[row1].Length - 1; i++) {
			//
			// The four points of this quad.
			//

			/**
			 *  3 - 2
			 *  | / |
			 *  1 - 0
			 */

			MapVertex[] quad = [
				points[row1][i + 1].Clone(),
				points[row1][i].Clone(),
				points[row2][i + 1].Clone(),
				points[row2][i].Clone(),
			];
			quadList.Add(quad);

			int page = this.TextureQuad(quad, this.Iam.SuperMap.DStyles[styleIndex], i, count);
			for (int j = 0; j < quad.Length; j++) {
				// @FIXME: I Think there is a bug here. We should create new Vertexes for the overlapping points
				//         or previous row texture gets replaced.
				quad[j].TexturePage = page;
			}

			//
			// Scale for block height.
			//

			// MASSIVE FUDGE.
			if (quad[0].UV.Y >= 0.9999f) {
				quad[0].UV = new Vector2(quad[0].UV.X, 0.0f);
			}
			if (quad[1].UV.Y >= 0.9999f) {
				quad[1].UV = new Vector2(quad[1].UV.X, 0.0f);
			}

			quad[2].UV = new Vector2(quad[2].UV.X, vheight);
			quad[3].UV = new Vector2(quad[3].UV.X, vheight);

			if (foundation == 2) {
				// @TODO:
				// quad[3].UV.Y = FacetDiffY[i];
				// quad[2].UV.Y = FacetDiffY[i + 1];

				//
				// OH MY GOD! FACET FOUNDATION TEXTURES WRAP!!!
				//

				// As a panic measure which might sort-of work, clamp the coords to 1.
				if (quad[3].UV.Y > 1.0f) {
					quad[3].UV = new Vector2(quad[3].UV.X, 1.0f);
				}
				if (quad[2].UV.Y > 1.0f) {
					quad[2].UV = new Vector2(quad[2].UV.X, 1.0f);
				}
			}
		}

		//	for (;i < count; i++)
		{
			_ = this.TextureRNG.Next();
		}

		return quadList;
	}

	private List<MapVertex[]> BuildCalls(DFacet facet, MapVertex[][] points, int count) {
		this.TextureRNG = new FacetTextureRNG((uint)facet.X[0].Value, (uint)facet.Y[0], (uint)facet.Z[0].Value);

		int styleIndex = facet.StyleIndex;
		if (facet.FacetFlags.IsSet(FacetFlag.TwoSided)) {
			styleIndex -= 1;
		}

		int styleIndexStep = 1;
		if (!facet.FacetFlags.IsSet(FacetFlag.HugFloor) && facet.FacetFlags.IsAnySet([FacetFlag.TwoTextured, FacetFlag.TwoSided])) {
			styleIndexStep = 2;
		}

		//
		// Do we have a foundation?
		//
		int foundation = 0;
		if (facet.FHeight != 0) {
			foundation = 2;
		}

		float blockHeight = facet.BlockHeight << 4;
		int height = facet.Height;

		//
		// Go through the facet and find all the quads that use
		// this call's texture.
		//

		var quadList = new List<MapVertex[]>();
		uint hf = 0;
		while (height >= 0) {
			if (hf != 0) {
				quadList.AddRange(
						this.FillFacetPoints(
						points,
						count,
						hf - 1,
						foundation + 1,
						styleIndex - 1,
						blockHeight
					)
				);
			}

			height -= 4;
			hf += 1;
			foundation -= 1;
			styleIndex += styleIndexStep;
		}

		return quadList;
	}

	private List<MapVertex[]> ConvertSuperFacet(DFacet facet) {
		if (facet.Dfcache == 0) {
			// @TODO: df->Dfcache = NIGHT_dfcache_create(facet);
		}

		_ = CalcDirection(facet);

		// How long is the wall? No diagonal walls allowed.
		var mapXLen = facet.X[1].ToMap().Value - facet.X[0].ToMap().Value;
		var mapZLen = facet.Z[1].ToMap().Value - facet.Z[0].ToMap().Value;

		if (mapXLen != 0 && mapZLen != 0) {
			GD.PushError($"Diagonal wall detected. {facet.X}, {facet.Z}");
		}

		var mapStartX = (float)facet.X[0].ToMap().Value;
		var mapStartY = (float)facet.Y[0];
		var mapStartZ = (float)facet.Z[0].ToMap().Value;

		// Number of points in the direction:
		// Facets are never diagonal, so you go on dx or on dz, never on both.
		// |           |
		// .__.__.__.__.
		int count;
		if (mapXLen != 0) {
			count = Mathf.Abs(new MapCoordinate(mapXLen).ToHighRes().Value);
		} else {
			count = Mathf.Abs(new MapCoordinate(mapZLen).ToHighRes().Value);
		}
		count += 1;

		var foundation = facet.FHeight != 0 ? 2 : 0;
		var blockHeight = facet.BlockHeight << 4;
		var height = facet.Height;

		this.SetupSuperfacetTextures(facet, count, height);

		this.TextureRNG = new FacetTextureRNG((uint)facet.X[0].Value, (uint)facet.Y[0], (uint)facet.Z[0].Value);
		// @TODO: SUPERFACET_colour_base = col = NIGHT_dfcache[df->Dfcache].colour;
		var points = this.CreateSuperFacetPoints(
			mapStartX, mapStartY, mapStartZ,
			(mapXLen == 0 ? 0 : (mapXLen > 0 ? 1 : -1)) * 256, (mapZLen == 0 ? 0 : (mapZLen > 0 ? 1 : -1)) * 256,
			blockHeight,
			height,
			new Color(),
			foundation,
			count,
			facet.FacetFlags.IsSet(FacetFlag.HugFloor)
		);

		return this.BuildCalls(facet, points, count);
	}

	private List<MapVertex[]> ConvertCommonFacet(DFacet facet, byte alpha) {
		if (facet.FacetFlags.IsSet(FacetFlag.Invisible)) {
			return [];
		}

		if (alpha != 0) {
			GD.PushError("ConvertCommonFacet expects alpha to always be 0, something is weird here...");
		}

		if (facet.Open == 0) {
			return this.ConvertSuperFacet(facet);
		}

		GD.PushWarning("@TODO: Facet Open code");
		return [];
	}

	private static void ConvertRareFacet(DFacet facet, byte alpha) {
		_ = facet;
		_ = alpha;
		GD.PushWarning("@TODO: Rare facet is skipped.");
	}

	public List<Facet> Convert() {
		this.LoadFacetsFromBuildings();
		var list = new List<Facet>();

		for (int z = 0; z < Iam.MapLoSize - 1; z++) {
			for (int x = 0; x < Iam.MapLoSize - 1; x++) {
				foreach (var facet in this.LoMapWho[x][z].Facet) {
					int buildingId = 0;
					DBuilding building = null;
					if (facet.FacetType == FacetType.Normal) {
						buildingId = facet.Building;
						building = this.Iam.SuperMap.DBuildings[buildingId];
					}

					if ((buildingId != 0 && building.Type == BuildingType.CrateIn) || facet.FacetFlags.IsSet(FacetFlag.Inside)) {
						// Don't draw inside buildings outside.
						GD.PushWarning("Inside is being skipped");
					} else if (facet.FacetType == FacetType.Door) {
						//
						// Draw the warehouse ground around this facet but don't draw
						// the facet.
						//

						// @TODO: AENG_draw_box_around_recessed_door(&dfacets[facet], FALSE);
						GD.PushWarning("Incomplete door");
					} else {
						if (IsRareFacet(facet)) {
							ConvertRareFacet(facet, 0);
						} else {
							list.Add(new Facet() { Quads = this.ConvertCommonFacet(facet, 0) });
						}

						if (facet.FacetType == FacetType.Normal && building != null) {
							GD.PushWarning("@TODO: Walkable");
							// @TODO: FACET_draw_walkable(buildingId);
						}
					}
				}
			}
		}

		return list;
	}
}
