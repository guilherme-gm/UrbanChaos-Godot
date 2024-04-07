using AssetTools.UCFileStructures;
using AssetTools.UCFileStructures.Maps;
using AssetTools.UCFileStructures.Maps.SuperMap;
using AssetTools.UCWorld.Maps.Converters.FacetTypes;
using AssetTools.UCWorld.Poly;
using AssetTools.UCWorld.Utils;
using Godot;
using System;
using System.Collections.Generic;

namespace AssetTools.UCWorld.Maps.Converters;

public class RareFacetConverter
{
	private readonly UCMap UCMap;

	private Iam Iam => this.UCMap.Iam.Data;

	private readonly FenceFlatAndOutsideDoorConverter FenceFlatAndOutsideDoorConverter;

	public RareFacetConverter(UCMap ucMap) {
		this.UCMap = ucMap;
		this.FenceFlatAndOutsideDoorConverter = new FenceFlatAndOutsideDoorConverter(ucMap);
	}

	private static void ConvertCable(DFacet facet) {
		// facet.cpp cable_draw
		GD.PushWarning("@TODO: Convert cable");
	}

	private static void ConvertLadder(DFacet facet) {
		GD.PushWarning("@TODO Convert ladder");
		// facet.cpp DRAW_ladder
	}

	private int GridHeightAt(HighResCoordinate highX, HighResCoordinate highZ) {
		return this.Iam.HighResMap[highX.Value][highZ.Value].Alt.ToMap().Value;
	}

	private int GridHeightAtWorld(float mapX, float mapZ) {
		return this.GridHeightAt(((MapCoordinate)mapX).ToHighRes(), ((MapCoordinate)mapZ).ToHighRes());
	}

	public List<QuadPoly> Convert(DFacet facet) {
		if (facet.FacetFlags.IsSet(FacetFlag.Invisible)) {
			return [];
		}

		if (facet.FacetType == FacetType.Cable) {
			ConvertCable(facet);
			return [];
		}

		if (facet.FacetType == FacetType.JustCollision) {
			// GD.Print($"Found a Collision Facet");
			return [];
		}

		if (facet.FacetType == FacetType.Oinside) {
			// @TODO: Understand whther OInside should or should not be converted here.
			//        The original code does a backface culling during rendering and runs draw_wall_thickness instead
			GD.PushWarning("There is a OInside facet. We may be drawing it wrongly...");
			// draw_wall_thickness
			return [];
		}

		if (facet.FacetType == FacetType.Ladder) {
			ConvertLadder(facet);
			return [];
		}

		int maxHeight = Int32.MaxValue;

		if (facet.Dfcache == 0) {
			// @TODO: Lightning cache
			// p_facet->Dfcache = NIGHT_dfcache_create(facet);
			// if (p_facet->Dfcache == NULL) {
			// return;
			// }
		}

		// @TODO:
		// NighColor col = NIGHT_dfcache[p_facet->Dfcache].colour;
		Color col = new Color();

		int dx = facet.X[1].ToMap().Value - facet.X[0].ToMap().Value;
		int dz = facet.Z[1].ToMap().Value - facet.Z[0].ToMap().Value;

		float sx = facet.X[0].ToMap().Value;
		float sy = facet.Y[0];
		float sz = facet.Z[0].ToMap().Value;

		var height = facet.Height;

		int count;
		float fdx, fdz;
		if (dx != 0 && dz != 0) {
			// Diagonal wall
			if (facet.FacetType == FacetType.Normal) {
				GD.PushError("Found a diagonal Normal facet. This should never happen...");
				return [];
			}

			int adx = Math.Abs(dx);
			int adz = Math.Abs(dz);
			int len = MathUtils.QDist2(adx, adz);
			count = len / 256;

			if (count == 0) {
				count = 1;
			}

			fdx = dx / (float)count;
			fdz = dz / (float)count;
		} else {
			if (dx != 0) {
				count = Math.Abs(dx) / 256;
				if (dx > 0) {
					dx = 256;
				} else {
					dx = -256;
				}
			} else {
				count = Math.Abs(dz) / 256;
				if (dz > 0) {
					dz = 256;
				} else {
					dz = -256;
				}
			}

			fdx = dx;
			fdz = dz;

			if (facet.Open != 0) {
				float angle = facet.Open * (MathUtils.PI / 256f);
				float rdx = ((float)Math.Cos(angle) * fdx) + ((float)Math.Sin(angle) * fdz);
				float rdz = (-(float)Math.Sin(angle) * fdx) + ((float)Math.Cos(angle) * fdz);

				fdx = rdx;
				fdz = rdz;
			}
		}

		count++;

		// Work out the height offset for each point along a fence.

		float[] diffY = new float[128];
		float x, y, z;
		if (facet.FacetType == FacetType.Fence
			|| facet.FacetType == FacetType.FenceFlat
			|| facet.FacetType == FacetType.FenceBrick
			|| facet.FacetType == FacetType.OutsideDoor
		) {
			x = sx;
			y = sy;
			z = sz;

			int idxDiffY = 0;
			int c0 = count;
			while (c0 > 0) {
				c0--;

				if (facet.FacetFlags.IsSet(FacetFlag.Onbuilding)) {
					// No offset for building facets.
					diffY[idxDiffY] = facet.Y[0];
					idxDiffY++;
				} else {
					// original code had a diag != 0 logic, but this never happens.
					float dy = this.GridHeightAtWorld(x, z);

					diffY[idxDiffY] = dy;
					idxDiffY++;

					x += fdx;
					z += fdz;
				}
			}
		}

		int styleIndex = facet.StyleIndex;
		if (facet.FacetType == FacetType.Cable) {
			GD.PushWarning("@TODO: cable_draw");
		} else if (facet.FacetType == FacetType.InsideDoor) {
			GD.PushWarning("@TODO: DRAW_door -- Inside Door");
		} else if (facet.FacetType == FacetType.Oinside || facet.FacetType == FacetType.Inside) {
			if (facet.FacetType == FacetType.Oinside) {
				styleIndex--;
			}

			GD.PushWarning("@TODO: Implement OInside / Insde logic");
		} else if (facet.FacetType == FacetType.Trench || facet.FacetType == FacetType.Normal || facet.FacetType == FacetType.Door) {
			GD.PushWarning("@TODO: Implement Trench / Normal / Door logic");
		} else if (facet.FacetType == FacetType.FenceBrick) {
			GD.PushWarning("@TODO: Implement FenceBrick logic");
		} else if (facet.FacetType == FacetType.Fence) {
			GD.PushWarning("@TODO: Implement Fence logic");
		} else if (facet.FacetType == FacetType.OutsideDoor || facet.FacetType == FacetType.FenceFlat) {
			return this.FenceFlatAndOutsideDoorConverter.Convert(
				facet, sx, sz, count, diffY, fdx, fdz, col, height, styleIndex
			);
		} else {
			GD.PushError($"Unexpected? Rare Face: {facet.FacetType}");
		}

		return [];
	}

}
