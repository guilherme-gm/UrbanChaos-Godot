namespace AssetTools.ImportTool.Commands;

public interface ICommand
{
	public void Execute();

	public string GetLog();
}
