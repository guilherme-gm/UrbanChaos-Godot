namespace AssetTools.ImportTool.Commands;

public class InitSection : ICommand
{
	public string SectionLog { get; set; }

	public InitSection(string log) {
		this.SectionLog = log;
	}

	public void Execute() {
		/* Does nothing -- this Command is meant for logging only */
	}

	public string GetLog() {
		return this.SectionLog;
	}
}
