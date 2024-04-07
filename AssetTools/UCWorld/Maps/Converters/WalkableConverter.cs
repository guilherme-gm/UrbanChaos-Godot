using AssetTools.UCFileStructures.Maps;
using AssetTools.UCFileStructures.Maps.SuperMap;
using Godot;
using System.Collections.Generic;

namespace AssetTools.UCWorld.Maps.Converters;

public class WalkableConverter
{
	private const int ROOF_SHIFT = 3;

	private readonly UCMap Map;

	private Iam Iam => this.Map.Iam.Data;

	public WalkableConverter(UCMap map) {
		this.Map = map;
	}

	private MapVertex PolyTransform(float px, float py, float pz) {
		// NOTE: This is assuming POLY_transform is just copying data.
		//       The original code for that is written in ASM
		//       and from its comments seems like it does clipping
		//       but I am not sure what else it does... so this is a blind guess
		return new MapVertex() {
			Position = new Vector3(px, py, pz),
			TexturePage = 1,
		};
	}

	// FACET_draw_walkable
	public List<Walkable> ConvertBuildingWalkables(DBuilding building) {
		bool isWarehouse = building.Type == BuildingType.Warehouse;

		if (isWarehouse) {
			// @TODO: rooftex = &WARE_rooftex[WARE_ware[p_dbuilding->Ware].rooftex];
		}

		var walkables = new List<Walkable>();

		int page = 0;
		DWalkable walkable;
		for (int walkableIdx = building.Walkable; walkableIdx != 0; walkableIdx = walkable.Next) {
			walkable = this.Iam.SuperMap.WalkablesSection.DWalkables[walkableIdx];

			var triangles = new List<MapVertex[]>();
			var quads = new List<MapVertex[]>();
			for (int i = walkable.StartFace4; i < walkable.EndFace4; i++ /* rooftex++ */) {
				var face4 = this.Iam.SuperMap.WalkablesSection.RoofFace4s[i];
				if (face4.DrawFlags.IsSet(RoofFace4DrawFlags.NoDraw)) {
					continue;
				}

				float px = face4.RX.Coordinate.ToMap().Value;
				float pz = face4.RZ.Coordinate.ToMap().Value;
				float py = face4.Y;

				float sy = py;

				var vertex1 = this.PolyTransform(px, py, pz);
				// @TODO: NIGHT_get_d3d_colour(NIGHT_ROOF_WALKABLE_POINT(i,0), &pp->colour, &pp->specular);
				// vertices.Add(vertex1);

				px += 256;
				py += face4.DY[0] << ROOF_SHIFT;
				var vertex2 = this.PolyTransform(px, py, pz);
				// @TODO: NIGHT_get_d3d_colour(NIGHT_ROOF_WALKABLE_POINT(i,1), &pp->colour, &pp->specular);
				// vertices.Add(vertex2);

				pz += 256;
				py = sy + (face4.DY[1] << ROOF_SHIFT);
				var vertex3 = this.PolyTransform(px, py, pz);
				// @TODO: NIGHT_get_d3d_colour(NIGHT_ROOF_WALKABLE_POINT(i,3), &pp->colour, &pp->specular);
				// vertices.Add(vertex3);

				px -= 256;
				py = sy + (face4.DY[2] << ROOF_SHIFT);
				var vertex4 = this.PolyTransform(px, py, pz);
				// @TODO: NIGHT_get_d3d_colour(NIGHT_ROOF_WALKABLE_POINT(i,2), &pp->colour, &pp->specular);
				// vertices.Add(vertex4);

				// Vertex 4 GOES BEFORE vertex 3. This is not a Typo
				MapVertex[] quad = [
					vertex1,
					vertex2,
					vertex4,
					vertex3,
				];

				if (isWarehouse) {
					GD.PushWarning("@TODO: Warehouse walkable");
				} else {
					var mapHi = this.Iam.HighResMap[face4.RX.Coordinate.Value][face4.RZ.Coordinate.Value];
					page = mapHi.Texture.SetupMinitextureUV(page, quad[0], quad[1], quad[2], quad[3]);
				}

				if (page > 1506) { // POLY_NUM_PAGES - 2
					page = 0;
				}

				if (!isWarehouse && face4.DrawFlags.IsAnySet([RoofFace4DrawFlags.Shadow1, RoofFace4DrawFlags.Shadow2, RoofFace4DrawFlags.Shadow3])) {
					GD.PushWarning("@TODO: Roof shadow");
				} else {
					if (face4.RX.DrawMode == 1) {
						triangles.Add([quad[0], quad[1], quad[3]]);
						triangles.Add([quad[3], quad[2], quad[0]]);

						// @TODO: Implement 2pass -- POLY_add_triangle
					} else {
						quads.Add(quad); // // 0, 1, 2 ; 3, 2, 1

						// @TODO: Implement 2pass -- POLY_add_quad
					}
				}
			}

			walkables.Add(new Walkable() {
				Quads = quads,
				Triangles = triangles,
			});
		}

		return walkables;
	}
}
