using AssetTools.Structures;
using AssetTools.UCFileStructures.Maps;
using Godot;
using System;
using System.IO;

namespace AssetTools.AssetManagers;

public class MapManager
{
	public static readonly MapManager Instance = new MapManager();

	public static string GetUCMapPath(string mapPath = "") {
		return Path.Join(AssetPathManager.Instance.UCFolderPath, mapPath);
	}

	public Map LoadMap(string mapPath) {
		var filePath = GetUCMapPath(mapPath);
		using var fs = new FileStream(filePath, FileMode.Open);
		using var br = new BinaryReader(fs);
		var iam = Iam.Deserialize(br);

		return new Map() {
			IamFile = iam,
			IamFilePath = mapPath,
			IamStatus = AssetLoadStatus.Loaded,
		};
	}

	public bool TryLoadMap(string mapPath, out Map map) {
		if (!File.Exists(GetUCMapPath(mapPath))) {
			map = new Map() {
				IamFile = null,
				IamFilePath = mapPath,
				IamStatus = AssetLoadStatus.NotFound,
			};
			return false;
		}

		try {
			map = this.LoadMap(mapPath);
			return true;
		}
		catch (Exception error) {
			GD.PushError(error);
			map = new Map() {
				IamFile = null,
				IamFilePath = mapPath,
				IamStatus = AssetLoadStatus.Error,
			};
			return false;
		}
	}
}
