using AssetTools.AssetManagers;
using System;
using System.IO;

namespace AssetTools.ImportTool.Commands;

public class CopyFile : ICommand
{
	private string From { get; set; }

	private string To { get; set; }

	public CopyFile(string from, string to) {
		this.From = from;
		this.To = to;
	}

	public void Execute() {
		var fromPath = Path.Join(AssetPathManager.Instance.UCFolderPath, this.From);
		var toPath = Path.Join(AssetPathManager.Instance.WorkFolderPath, this.To);

		if (!File.Exists(fromPath)) {
			throw new Exception($"Could not find source file \"{fromPath}\"");
		}

		_ = Directory.CreateDirectory(Path.GetDirectoryName(toPath));

		File.Copy(fromPath, toPath, true);
	}

	public string GetLog() {
		return $"Copying \"{this.From}\" to \"{this.To}\"";
	}
}
