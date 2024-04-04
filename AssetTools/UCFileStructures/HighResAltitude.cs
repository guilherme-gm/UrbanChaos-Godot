using AssetTools.UCWorld;

namespace AssetTools.UCFileStructures;

public record HighResAltitude(int Value)
{
	public MapAltitude ToMap() {
		// << 3
		return new MapAltitude(this.Value << 3);
	}

	public static explicit operator HighResAltitude(int value) {
		return new HighResAltitude(value);
	}

	public override string ToString() => $"HAlt({this.Value})";
}
