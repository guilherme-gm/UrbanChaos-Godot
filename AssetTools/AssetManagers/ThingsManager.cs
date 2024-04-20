using AssetTools.UCWorld.Things;
using Godot;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AssetTools.AssetManagers;

public class ThingsManager
{
	public class ThingFileInfo
	{
		public string FileName { get; set; }

		/**
		 * Whether this Prim is usually ignored in PC.
		 * Usually, a Prim is ignored when it has a equivalent NPrim (New Prim)
		 */
		public bool IsIgnored { get; set; }
	}

	public static readonly ThingsManager Instance = new ThingsManager();

	public static string GetUCThingPath(string thingName = "") {
		return Path.Join(AssetPathManager.Instance.UCFolderPath, "data", thingName);
	}

	public ThingFileInfo[] ListThings() {
		var loadedThings = new HashSet<string>();
		var thingFiles = new List<ThingFileInfo>();

		var folderPath = GetUCThingPath();
		var thingList = Directory
			.GetFiles(folderPath, "*.all", SearchOption.AllDirectories)
			.Select((file) => Path.GetRelativePath(folderPath, file))
			.ToArray();

		foreach (var fileName in thingList) {
			if (!loadedThings.Add(fileName.ToLower())) {
				GD.PrintErr($"Duplicated Thing {fileName} found.");
				continue;
			}

			thingFiles.Add(new ThingFileInfo() {
				FileName = fileName,
				IsIgnored = false,
			});
		}

		return [.. thingFiles];
	}

	public Thing LoadThing(string name) {
		var loader = new ThingLoader();
		return loader.LoadFomFile(GetUCThingPath(name));
	}
}
