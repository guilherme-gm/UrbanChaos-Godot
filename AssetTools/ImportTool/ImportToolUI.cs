using AssetTools.AssetManagers;
using Godot;
using System.Threading.Tasks;

namespace AssetTools.ImportTool;

public partial class ImportToolUI : Panel
{
	[Export]
	private FileDialog FileDialog { get; set; }

	[Export]
	private LineEdit GDInput { get; set; }

	[Export]
	private LineEdit UCInput { get; set; }

	[Export]
	private LineEdit UCGodotInput { get; set; }

	[Export]
	private Button ImportButton { get; set; }

	[Export]
	private Label ProgressLabel { get; set; }

	[Export]
	private ProgressBar ProgressBar { get; set; }

	[Export]
	private RichTextLabel Log { get; set; }

	private ImportTool ImportToolCore { get; set; }

	public override void _Ready() {
		this.ImportToolCore = new ImportTool();
		this.ImportToolCore.OnProgress += (int step, int total) => {
			_ = this.CallDeferred(nameof(this.OnImportToolCoreProgress), step, total);
		};
		this.ImportToolCore.OnProgressLog += (string message) => {
			_ = this.CallDeferred(nameof(this.OnImportToolCoreProgressLog), message);
		};
	}

	private void OnImportToolCoreProgress(int step, int total) {
		this.ProgressBar.MaxValue = total;
		this.ProgressBar.Step = step;
	}

	private void OnImportToolCoreProgressLog(string message) {
		this.Log.AppendText(message + "\n");
	}

	private void OnGDSearchBtnPressed() {
		this.FileDialog.Title = "Godot executable";
		this.FileDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
		this.FileDialog.SetMeta("target", this.GDInput);
		this.FileDialog.Show();
	}

	private void OnUCSearchBtnPressed() {
		this.FileDialog.Title = "Urban Chaos (Steam/GOG) Folder";
		this.FileDialog.FileMode = FileDialog.FileModeEnum.OpenDir;
		this.FileDialog.SetMeta("target", this.UCInput);
		this.FileDialog.Show();
	}

	private void OnUCGodotSearchBtnPressed() {
		this.FileDialog.Title = "UC Godot Folder";
		this.FileDialog.FileMode = FileDialog.FileModeEnum.OpenDir;
		this.FileDialog.SetMeta("target", this.UCGodotInput);
		this.FileDialog.Show();
	}

	private void OnFileDialogDirSelected(string dir) {
		this.FileDialog.GetMeta("target").As<LineEdit>().Text = dir;
	}

	private void SetImportElementsEnabled(bool enabled) {
		foreach (var element in this.GetTree().GetNodesInGroup("ImportInfoElements")) {
			if (element is Button btnEle) {
				btnEle.Disabled = !enabled;
			}
			if (element is LineEdit lineEle) {
				lineEle.Editable = enabled;
			}
		}
	}

	private async void OnImportBtnPressed() {
		try {
			this.Log.Clear();
			this.SetImportElementsEnabled(false);

			this.ProgressLabel.Text = "Importing...";
			this.ProgressBar.Step = 0;

			AssetPathManager.Instance.SetPaths(this.GDInput.Text, this.UCInput.Text, this.UCGodotInput.Text);
			await Task.Run(this.ImportToolCore.Run);

			this.ProgressLabel.Text = "Completed.";
		}
		finally {
			this.SetImportElementsEnabled(true);
		}
	}

}
