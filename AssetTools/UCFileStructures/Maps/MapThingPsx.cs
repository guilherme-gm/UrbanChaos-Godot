namespace AssetTools.UCFileStructures.Maps;

[Deserializer.DeserializeGenerator]
public partial class MapThingPsx
{
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
	public ushort MapChild { get; set; }   //mapwho 2 way linked list
	public ushort MapParent { get; set; }
	public byte Type { get; set; }
	public byte SubType { get; set; }  // Type for lights...
	public uint Flags { get; set; }

	public short IndexOther { get; set; }       // Brightness for lights...
	public ushort Width { get; set; }
	public ushort Height { get; set; }
	public ushort IndexOrig { get; set; }        // param for lights...
	public ushort AngleX { get; set; }           // (R,G,B) for lights...
	public ushort AngleY { get; set; }
	public ushort AngleZ { get; set; }
	public ushort IndexNext { get; set; }
	public short LinkSame { get; set; }
	public short OnFace { get; set; }
	public short State { get; set; }
	public short SubState { get; set; }
	public int BuildingList { get; set; }
	public uint EditorFlags { get; set; }
	public uint EditorData { get; set; }
	[Deserializer.FixedArray(Dimensions = [3])]
	public uint[] DummyArea { get; set; }
	public int TweenStage { get; set; }
	//struct KeyFrameElement		*AnimElements,
	//								*NextAnimElements;
	public int CurrentFramePtr { get; set; } // KeyFrame* CurrentFrame;
	public int NextFramePtr { get; set; } // KeyFrame* NextFrame;

}
