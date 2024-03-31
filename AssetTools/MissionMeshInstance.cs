using AssetTools.AssetManagers;
using AssetTools.Structures;
using AssetTools.UCFileStructures.Maps;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AssetTools;

[Tool]
public partial class MissionMeshInstance : MeshInstance3D
{
	private struct Realloc
	{
		public float XOffset;
		public float YOffset;
		public float XMultiplier;
		public float YMultiplier;

		public override readonly string ToString() {
			return $"Offset = ({this.XOffset}, {this.YOffset}) ; Multiplier = ({this.XMultiplier}, {this.YMultiplier})";
		}
	}

	private string MissionFilePath;

	private Dictionary<int, Realloc> MaterialRealloc { get; set; }
	private Material TheMaterial { get; set; }

	[Export(PropertyHint.GlobalFile)]
	public string MissionPath {
		get => this.MissionFilePath;
		set {
			this.MissionFilePath = value;
			this.LoadMission(this.MissionFilePath, "testdrive1a.txc");
		}
	}

	private Vector2 ReallocUV(Vector2 originalUV, Realloc reallocData) {
		// return originalUV;
		var res = new Vector2(
			(originalUV.X * reallocData.XMultiplier) + reallocData.XOffset,
			(originalUV.Y * reallocData.YMultiplier) + reallocData.YOffset
		);

		return res;
	}

	private void DrawMission(Mission mission) {
		SurfaceTool st = new SurfaceTool();


		st.Begin(Mesh.PrimitiveType.Triangles);
		st.SetMaterial(this.TheMaterial);

		int vertexCount = 0;
		foreach (var floor in mission.Map.FloorStores) {
			// @TODO: Implement shadows
			var isShadow = floor.Flags.IsAnySet([MapFlag.Shadow1, MapFlag.Shadow2, MapFlag.Shadow3]);
			if (isShadow) {
				GD.Print($"Skipping shadows");
				continue;
			}

			if (floor.Flags.IsSet(MapFlag.RoofExists)) {
				GD.Print("There are roofs");
				// @TODO: y = floor.Height;
			}

			var dy = 0f;
			if (floor.Flags.IsSet(MapFlag.SinkSquare) /* && !isWarehouse */) {
				GD.Print("There are SinkSquare");
				dy = -32f;
			}

			foreach (var idx in new int[] { 0, 1, 2, 3 }) {
				var uv = floor.UVs[idx];
				var realloc = this.MaterialRealloc[floor.TexturePage];
				st.SetUV(this.ReallocUV(uv, realloc));
				st.SetNormal(Vector3.Up);
				st.AddVertex(floor.Vertices[idx] + new Vector3(0, dy, 0));
				vertexCount++;
			}

			var start = vertexCount - 4;

			st.AddIndex(start + 0);
			st.AddIndex(start + 1);
			st.AddIndex(start + 3);

			st.AddIndex(start + 1);
			st.AddIndex(start + 2);
			st.AddIndex(start + 3);

		}

		this.Mesh = st.Commit();
	}

	private void LoadMaterials(Mission mission, string textureSet) {
		this.MaterialRealloc = [];
		var textures = mission.Map.FloorStores
			.Select((floor) => floor.TexturePage)
			.Distinct()
			.ToArray();

		var size = (int)Math.Ceiling(Math.Sqrt(textures.Length));
		var textureImage = Image.Create(size * 64, size * 64, false, Image.Format.Rgba8);

		var posX = 0;
		var posY = 0;

		this.MaterialRealloc = [];

		foreach (var textureId in textures) {
			var texturePath = TextureManager.GetWorkDirPath(textureSet, $"tex{textureId.ToString().PadLeft(3, '0')}.tga");
			if (!File.Exists(texturePath)) {
				GD.PushWarning($"Could not find texture: ${texturePath}");
				continue;
			}

			var image = new Image();
			if (image.Load(texturePath) != Error.Ok) {
				GD.PushError("Failed to load image");
				continue;
			}

			var quadX = posX * 64;
			var quadY = posY * 64;
			for (int x = 0; x < 64; x++) {
				for (int y = 0; y < 64; y++) {
					var color = image.GetPixel(x, y);
					textureImage.SetPixel(quadX + x, quadY + y, color);
				}
			}

			var realloc = new Realloc() {
				XMultiplier = 1f / size,
				YMultiplier = 1f / size,
				XOffset = posX * (1f / size),
				YOffset = posY * (1f / size),
			};
			this.MaterialRealloc.Add(textureId, realloc);

			posX++;
			if (posX == size) {
				posX = 0;
				posY++;
			}
		}

		var texture = new ImageTexture();
		texture.SetImage(textureImage);

		var material = new StandardMaterial3D {
			AlbedoTexture = texture
		};

		this.TheMaterial = material;
	}

	public void LoadMission(string path, string textureSet = "") {
		if (path == "") {
			return;
		}

		this.MissionFilePath = path;

		var fileName = Path.GetFileName(path).ToLower();
		Mission mission;

		mission = MissionsManager.Instance.LoadMission(fileName);

		if (textureSet != "") {
			this.LoadMaterials(mission, textureSet);
		} else {
			this.MaterialRealloc = null;
		}

		this.DrawMission(mission);
	}

	public void LoadMission(Mission mission, string textureSet = "") {
		this.MissionFilePath = null;
		if (textureSet != "") {
			this.LoadMaterials(mission, textureSet);
		} else {
			this.MaterialRealloc = null;
		}

		this.DrawMission(mission);

		foreach (var child in this.GetChildren()) {
			child.Free();
		}

		foreach (var mo in mission.Map.MapObjects) {
			var path = PrimsManager.GetUCPrimPath(mo.Prim);
			var primMesh = new PrimMeshInstance();
			primMesh.LoadPrim(path, textureSet);
			primMesh.Position = mo.Position;

			this.AddChild(primMesh);
		}
	}
}
