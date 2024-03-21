using Godot;

namespace AssetTools.ImportTool;

public partial class ImportTool : Resource
{
	[Signal]
	public delegate void OnProgressEventHandler(int step, int total);

	[Signal]
	public delegate void OnProgressLogEventHandler(string message);

	public void Run() {
		_ = this.EmitSignal(SignalName.OnProgressLog, "Done");
		_ = this.EmitSignal(SignalName.OnProgress, 3, 3);
	}
}
