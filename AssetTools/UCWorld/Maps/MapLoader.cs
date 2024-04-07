using AssetTools.AssetManagers;
using AssetTools.Structures;
using AssetTools.UCFileStructures.Maps;
using AssetTools.UCWorld.Maps.Converters;
using Godot;
using System;
using System.IO;

namespace AssetTools.UCWorld.Maps;

/// <summary>
/// Handles the loading of a IAM (Urban Chaos Map) into a UCMap
/// </summary>
public class MapLoader
{
	private UCMap Map;

	private void ConvertMapData() {
		// 1. All coordinates must be in Map Coordinate
		// 2. All textures must be in screen-ready state (in *256 mode)
		//    - Texture pages must be right
		//    - Texture UV must be right
		//    - Vertex must be right, already flipped if needed
		// 3. Structures must be in a digestable format
		this.Map.TextureSet = TmaManager.Instance.LoadFile(this.Map.Iam.Data.TextureSet);

		var floorConverter = new FloorConverter(this.Map);
		this.Map.FloorFaces = floorConverter.Convert();

		var facetConverter = new FacetsConverter(this.Map);
		facetConverter.Convert();

		this.Map.Facets = facetConverter.ConvertedFacets;
		this.Map.Walkables = facetConverter.ConvertedWalkables;
	}

	public UCMap LoadFomFile(string path) {
		this.Map = new UCMap();

		try {
			if (!File.Exists(path)) {
				this.Map.Iam = new FileContainer<Iam>(null, path, AssetLoadStatus.NotFound);
				return this.Map;
			}

			using var fs = new FileStream(path, FileMode.Open);
			using var br = new BinaryReader(fs);

			var iam = Iam.Deserialize(br);
			this.Map.Iam = new FileContainer<Iam>(iam, path, AssetLoadStatus.Loaded);

			this.ConvertMapData();

			return this.Map;
		}
		catch (Exception exception) {
			GD.PushError($"Failed to load map file. {exception.Message}");
			GD.PushError(exception);
			if (this.Map.Iam == null) {
				this.Map.Iam = new FileContainer<Iam>(null, path, AssetLoadStatus.Error);
			} else {
				this.Map.Iam.Status = AssetLoadStatus.Error;
			}
			return this.Map;
		}
	}
}
