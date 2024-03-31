namespace AssetTools.UCFileStructures;

public record MapCoordinate(int Value)
{
	public HighResCoordinate ToHighRes() {
		// >> 8
		return new HighResCoordinate(this.Value / 256);
	}

	public LowResCoordinate ToLowRes() {
		return this.ToHighRes().ToLowRes();
	}

	public static explicit operator MapCoordinate(int value) {
		return new MapCoordinate(value);
	}

	public override string ToString() => $"MaC({this.Value})";
}
