namespace AssetTools.UCWorld.Utils;

public class GameRNG
{
	public static GameRNG Instance = new GameRNG(0);

	public uint Seed { get; set; }

	public GameRNG(uint seed) {
		this.Seed = seed;
	}

	public ushort Random() {
		this.Seed = (this.Seed * 69069) + 1;

		return (ushort)(this.Seed >> 7);
	}
}
