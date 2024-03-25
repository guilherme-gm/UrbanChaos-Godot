namespace AssetTools.Structures;

public record AssetLoadStatus(string Description, string ColorHex)
{
	public static AssetLoadStatus NotLoaded { get; } = new("Not loaded", "666666");
	public static AssetLoadStatus NotFound { get; } = new("Not found", "FCF36D");
	public static AssetLoadStatus Error { get; } = new("Loading error", "FF3B3B");
	public static AssetLoadStatus Loaded { get; } = new("Loaded", "FEFEFE");
}
