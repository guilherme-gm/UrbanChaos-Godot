using AssetTools.UCWorld;

namespace AssetTools.UCFileStructures;

public record LowResCoordinate(int Value)
{
	public HighResCoordinate ToHighRes() {
		// << 2
		return new HighResCoordinate(this.Value * 4);
	}

	public MapCoordinate ToMap() {
		return this.ToHighRes().ToMap();
	}

	public static explicit operator LowResCoordinate(int value) {
		return new LowResCoordinate(value);
	}

	public override string ToString() => $"LoC({this.Value})";
}
