namespace AssetTools.UCFileStructures.Maps.SuperMap;

public record HighResRoofCoordinate(HighResCoordinate Coordinate, byte DrawMode)
{
	public override string ToString() => $"HRoofC({this.Coordinate}, {this.DrawMode})";
}
