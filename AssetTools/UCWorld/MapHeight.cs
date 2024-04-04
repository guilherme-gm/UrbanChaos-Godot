using AssetTools.UCFileStructures;

namespace AssetTools.UCWorld;

public record MapHeight(int Value)
{
	public HighResHeight ToHighRes() {
		// >> 6
		return new HighResHeight(this.Value >> 6);
	}

	public static explicit operator MapHeight(int value) {
		return new MapHeight(value);
	}

	public override string ToString() => $"MHeight({this.Value})";
}
