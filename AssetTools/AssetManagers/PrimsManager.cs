// using AssetTools.UCFileStructures;
using Godot;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AssetTools.AssetManagers;

public class PrimsManager
{
	public class PrimFileInfo
	{
		public string FileName { get; set; }

		/**
		 * Whether this Prim is usually ignored in PC.
		 * Usually, a Prim is ignored when it has a equivalent NPrim (New Prim)
		 */
		public bool IsIgnored { get; set; }
	}

	public static readonly PrimsManager Instance = new PrimsManager();

	public static string GetUCPrimPath(string primName = "") {
		return Path.Join(AssetPathManager.Instance.UCPath, "server", "prims", primName);
	}

	public PrimFileInfo[] ListPrims() {
		var loadedPrims = new HashSet<string>();
		var primFiles = new List<PrimFileInfo>();

		var folderPath = GetUCPrimPath();
		var nprimList = Directory
			.GetFiles(folderPath, "nprim*.prm", SearchOption.AllDirectories)
			.Select((file) => Path.GetRelativePath(folderPath, file))
			.ToArray();

		foreach (var fileName in nprimList) {
			if (!loadedPrims.Add(fileName.ToLower())) {
				GD.PrintErr($"Duplicated NPrim {fileName} found.");
				continue;
			}

			primFiles.Add(new PrimFileInfo() {
				FileName = fileName,
				IsIgnored = false,
			});
		}

		var primList = Directory
			.GetFiles(folderPath, "prim*.prm", SearchOption.AllDirectories)
			.Select((file) => Path.GetRelativePath(folderPath, file))
			.ToArray();

		foreach (var fileName in primList) {
			if (!loadedPrims.Add(fileName.ToLower())) {
				GD.PrintErr($"Duplicated Prim {fileName} found.");
				continue;
			}

			var isIgnored = loadedPrims.Contains($"n{fileName.ToLower()}");

			primFiles.Add(new PrimFileInfo() {
				FileName = fileName,
				IsIgnored = isIgnored,
			});
		}

		return [.. primFiles];
	}
}
