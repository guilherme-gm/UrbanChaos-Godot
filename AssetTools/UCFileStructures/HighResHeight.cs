using AssetTools.UCWorld;

namespace AssetTools.UCFileStructures;

public record HighResHeight(int Value)
{
	public MapHeight ToMap() {
		// << 6
		return new MapHeight(this.Value << 6);
	}

	public static explicit operator HighResHeight(int value) {
		return new HighResHeight(value);
	}

	public override string ToString() => $"HHeight({this.Value})";
}
