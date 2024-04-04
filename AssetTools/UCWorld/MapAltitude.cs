using AssetTools.UCFileStructures;

namespace AssetTools.UCWorld;

public record MapAltitude(int Value)
{
	public HighResAltitude ToHighRes() {
		// >> 3
		return new HighResAltitude(this.Value >> 3);
	}

	public static explicit operator MapAltitude(int value) {
		return new MapAltitude(value);
	}

	public override string ToString() => $"MAlt({this.Value})";
}
