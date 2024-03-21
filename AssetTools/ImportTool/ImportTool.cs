using AssetTools.ImportTool.Commands;
using Godot;
using System.Collections.Generic;

namespace AssetTools.ImportTool;

public partial class ImportTool : Resource
{
	[Signal]
	public delegate void OnProgressEventHandler(int current, int total);

	[Signal]
	public delegate void OnProgressLogEventHandler(string message);

	private List<ICommand> Commands;

	public void InitCommands() {
		this.Commands = [
			new CreateProject(),

			new InitSection("Importing textures..."),
			new CopyFile("/data/TITLE LEAVES1.TGA", "/Assets/MainMenu/BG/Leaves_Main.tga"),
			new CopyFile("/data/map leaves darci.tga", "/Assets/MainMenu/BG/Leaves_Map_Darci.tga"),
			new CopyFile("/data/briefing leaves darci.tga", "/Assets/MainMenu/BG/Leaves_Briefing_Darci.tga"),
			new CopyFile("/data/config leaves.tga", "/Assets/MainMenu/BG/Leaves_Config.tga"),

			new PackProject(),
			new DeleteTemp(),
		];
	}

	public void Run() {
		this.InitCommands();

		_ = this.EmitSignal(SignalName.OnProgress, 0, this.Commands.Count);

		int progress = 0;
		foreach (var command in this.Commands) {
			_ = this.EmitSignal(SignalName.OnProgressLog, command.GetLog());
			command.Execute();

			progress++;
			_ = this.EmitSignal(SignalName.OnProgress, progress, this.Commands.Count);
		}

		_ = this.EmitSignal(SignalName.OnProgressLog, "Done");
	}
}
