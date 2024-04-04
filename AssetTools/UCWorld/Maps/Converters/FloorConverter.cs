using AssetTools.UCFileStructures;
using AssetTools.UCFileStructures.Maps;
using AssetTools.Utils;
using Godot;
using System.Collections.Generic;

namespace AssetTools.UCWorld.Maps.Converters;

public class FloorConverter
{
	private struct FloorCell
	{
		public float Alt;
		public Flags<MapFlag> Flags;
		public CompressedTextureInfo Texture;

		public Color Color;
	}

	private readonly UCMap UCMap;

	private Iam Iam => this.UCMap.Iam.Data;

	private FloorCell[][] FloorCells;

	public FloorConverter(UCMap ucMap) {
		this.UCMap = ucMap;
	}

	private int SetupMinitextureUV(CompressedTextureInfo texture, int page, MapVertex v0, MapVertex v1, MapVertex v2, MapVertex v3) {
		v0.UV = texture.UVs[0];
		v1.UV = texture.UVs[1];
		v2.UV = texture.UVs[2];
		v3.UV = texture.UVs[3];

		if (page > 1408) { // TEXTURE_NUM_STANDARD
			return 0;
		} else {
			return page;
		}
	}

	private void GenerateFloorCells() {
		this.FloorCells = new FloorCell[this.Iam.HighResMap.Length][];
		// Convert floor -- cache_a_row
		for (int x = 0; x < this.Iam.HighResMap.Length; x++) {
			this.FloorCells[x] = new FloorCell[this.Iam.HighResMap[x].Length];

			for (int z = 0; z < this.Iam.HighResMap[x].Length; z++) {
				var cell = this.Iam.HighResMap[x][z];
				this.FloorCells[x][z] = new FloorCell() {
					Alt = cell.Alt.ToMap().Value,
					Flags = cell.Flags,
					Texture = cell.Texture,
				};

				// @TODO: Get night stuff
			}
		}
	}

	private MapVertex[] MakeVertices(int x, int z) {
		var vertices = new MapVertex[4];
		vertices[0] = new MapVertex() {
			Position = new Vector3(x * 256, 0, z * 256),
			Color = this.FloorCells[x][z].Color,
			Specular = 0xff000000,
		};
		vertices[1] = new MapVertex() {
			Position = new Vector3((x + 1) * 256, 0, z * 256),
			Color = this.FloorCells[x][z + 1].Color,
			Specular = 0xff000000,
		};
		vertices[2] = new MapVertex() {
			Position = new Vector3((x + 1) * 256, 0, (z + 1) * 256),
			Color = this.FloorCells[x + 1][z + 1].Color,
			Specular = 0xff000000,
		};
		vertices[3] = new MapVertex() {
			Position = new Vector3(x * 256, 0, (z + 1) * 256),
			Color = this.FloorCells[x + 1][z].Color,
			Specular = 0xff000000,
		};

		return vertices;
	}

	public List<FloorFace> Convert() {
		this.GenerateFloorCells();

		bool isWarehouse = false;
		var floorFaces = new List<FloorFace>();
		for (int z = 0; z < 128 - 1; z++) {
			for (int x = 0; x < 128 - 1; x++) {
				var floor1 = this.FloorCells[x][z];
				var floor2 = this.FloorCells[x + 1][z];
				// @TODO: Warehouse stuff

				// @TODO: Kerbs stuff

				if (isWarehouse && floor1.Flags.IsSet(MapFlag.Hidden)) {
					continue;
				}

				int page = floor1.Texture.TexturePage;
				if (page == (4 * 64) + 53 /* 309 */) {
					// @TODO: general_steam(x, z, floorStore1->Texture, 1); // store it
				}

				int dy = 0;
				if (!isWarehouse && floor1.Flags.IsSet(MapFlag.SinkSquare)) {
					dy = -32; // KERB_HEIGHT
				}

				var vertices = this.MakeVertices(x, z);

				// 3 and 2 are intentionally swapped!
				_ = this.SetupMinitextureUV(floor1.Texture, 0, vertices[0], vertices[1], vertices[3], vertices[2]);

				if (!isWarehouse && floor1.Flags.IsSet(MapFlag.RoofExists)) {
					float y = this.Iam.HighResMap[x][z].Height.ToMap().Value;
					vertices[0].Position += y * Vector3.Up;
					vertices[1].Position += y * Vector3.Up;
					vertices[2].Position += y * Vector3.Up;
					vertices[3].Position += y * Vector3.Up;
				} else {
					vertices[0].Position += (floor1.Alt + dy) * Vector3.Up;
					vertices[1].Position += (this.FloorCells[x][z + 1].Alt + dy) * Vector3.Up;
					vertices[2].Position += (this.FloorCells[x + 1][z + 1].Alt + dy) * Vector3.Up;
					vertices[3].Position += (floor2.Alt + dy) * Vector3.Up;
				}

				bool isShadow = floor1.Flags.IsAnySet([MapFlag.Shadow1, MapFlag.Shadow2, MapFlag.Shadow3]);
				if (isShadow) {
					// @TODO: Shadow
				} else {
					floorFaces.Add(new FloorFace() {
						TexturePage = page,
						// 3 and 2 intentionally swapped.
						Vertices = [vertices[0], vertices[1], vertices[3], vertices[2]],
					});
				}
			}
		}

		return floorFaces;
	}
}
