using AssetTools.UCWorld;

namespace AssetTools.UCFileStructures;

public record HighResCoordinate(int Value)
{
	public LowResCoordinate ToLowRes() {
		// >> 2
		return new LowResCoordinate(this.Value / 4);
	}

	public MapCoordinate ToMap() {
		// << 8
		return new MapCoordinate(this.Value * 256);
	}

	public static explicit operator HighResCoordinate(int value) {
		return new HighResCoordinate(value);
	}

	public override string ToString() => $"HiC({this.Value})";
}
