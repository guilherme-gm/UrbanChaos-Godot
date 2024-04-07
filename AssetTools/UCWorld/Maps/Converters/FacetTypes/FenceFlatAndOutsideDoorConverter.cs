using AssetTools.UCFileStructures.Maps;
using AssetTools.UCFileStructures.Maps.SuperMap;
using AssetTools.UCWorld.Poly;
using AssetTools.UCWorld.Textures;
using AssetTools.UCWorld.Utils;
using Godot;
using System.Collections.Generic;

namespace AssetTools.UCWorld.Maps.Converters.FacetTypes;

public class FenceFlatAndOutsideDoorConverter
{
	private readonly float[] ShakeTable = [
		0,0,0,0,
		+1,-2,+1,-1,
		+3,-3,+2,-2,
		+4,-3,+2,-4,
		+5,-5,+5,-4,
		+6,-4,+7,-6,
		+7,-6,+8,-9,
		+9,-7,+8,-9,
	];

	private FacetTextureRNG TextureRNG;

	private readonly UCMap UCMap;

	private Iam Iam => this.UCMap.Iam.Data;

	public FenceFlatAndOutsideDoorConverter(UCMap map) {
		this.UCMap = map;
	}


	public List<QuadPoly> Convert(DFacet facet, float sx, float sz, int count, float[] diffY, float fdx, float fdz, Color col, int height, int styleIndex) {
		_ = col;

		this.TextureRNG = new FacetTextureRNG((uint)facet.X[0].Value, (uint)facet.Y[0], (uint)facet.Z[0].Value);

		int hf = 0;
		uint shake = facet.Shake;

		if (shake != 0) {
			shake -= shake / 4;
			shake -= 1;
		}

		// fences DO lock to the floor
		int sy = 0;
		float y = sy;

		int[] rowStartVerticeIdx = new int[2];
		float x, z;

		var vertices = new List<MapVertex>();
		while (hf <= 1) {
			x = sx;
			z = sz;
			int idxDiffY = 0;
			rowStartVerticeIdx[hf] = vertices.Count;
			for (int c0 = count; c0 > 0; c0--) {
				float dy = diffY[idxDiffY];
				idxDiffY++;

				MapVertex pp = new MapVertex() {
					Position = new Vector3(
						x + this.ShakeTable[(GameRNG.Instance.Random() & 3) + ((shake / 8) & ~0x3)],
						y + dy,
						z + this.ShakeTable[(GameRNG.Instance.Random() & 3) + ((shake / 8) & ~0x3)]
					)
				};

				// @TODO: Coloring -- NIGHT_get_d3d_colour(*col, &pp->colour, &pp->specular);
				// 	pp->colour |= fade_alpha;

				vertices.Add(pp);

				x += fdx;
				z += fdz;
				// col += 1;
			}

			if (height == 2) {
				y += 102; //256.0/3.0;
			} else {
				y += height * 64; /* BLOCK_SIZE */
			}

			hf++;
		}

		int row1 = rowStartVerticeIdx[0];
		int row2 = rowStartVerticeIdx[1];

		//
		// create the quads and submit them for drawing
		//

		var list = new List<QuadPoly>();
		for (int c0 = 0; c0 < count - 1; c0++) {
			var quad = new MapVertex[] {
				vertices[row2 + c0 + 1].Clone(),
				vertices[row2 + c0].Clone(),
				vertices[row1 + c0 + 1].Clone(),
				vertices[row1 + c0].Clone()
			};

			//
			// Texture the quad.
			//

			int page = TextureUtils.TextureQuad(this.TextureRNG, this.UCMap, quad, this.Iam.SuperMap.DStyles[styleIndex], c0, count);
			var quadPoly = new QuadPoly(quad[0], quad[1], quad[2], quad[3], page);
			list.Add(quadPoly);
		}

		return list;
	}
}
