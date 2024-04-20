namespace AssetTools.UCFileStructures.MultiPrim;

[Deserializer.DeserializeGenerator]
public partial class GameKeyFrame
{
	public byte XYZIndex { get; set; }
	public byte TweenStep { get; set; }
	public short Flags { get; set; }
	public int FirstElementIdx { get; set; }

	[Deserializer.Skip]
	public GameKeyFrameElement FirstElement { get; set; } = null;

	public int PrevFrameIdx { get; set; }

	[Deserializer.Skip]
	public GameKeyFrame PrevFrame { get; set; } = null;

	public int NextFrameIdx { get; set; }

	[Deserializer.Skip]
	public GameKeyFrame NextFrame { get; set; } = null;

	public int FightIdx { get; set; }

	[Deserializer.Skip]
	public GameFightCol Fight { get; set; } = null;
}
