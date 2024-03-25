namespace AssetTools.UCFileStructures.Maps;

[Deserializer.DeserializeGenerator]
public partial class LoadGameThing
{
	public ushort Type { get; set; }
	public ushort SubStype { get; set; }

	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
	public int Flags { get; set; }

	public ushort IndexOther { get; set; }
	public ushort AngleX { get; set; }

	public ushort AngleY { get; set; }
	public ushort AngleZ { get; set; }

	[Deserializer.FixedArray(Dimensions = [4])]
	public int[] Dummy { get; set; }

}
