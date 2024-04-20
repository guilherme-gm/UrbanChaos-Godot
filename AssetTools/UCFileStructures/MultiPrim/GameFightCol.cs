namespace AssetTools.UCFileStructures.MultiPrim;

[Deserializer.DeserializeGenerator]
public partial class GameFightCol
{
	public ushort Dist1 { get; set; }
	public ushort Dist2 { get; set; }

	public byte Angle { get; set; }
	public byte Priority { get; set; }
	public byte Damage { get; set; }
	public byte Tween { get; set; }

	public byte AngleHitFrom { get; set; }
	public byte Height { get; set; }
	public byte Width { get; set; }
	public byte Dummy { get; set; }

	public int NextIdx { get; set; }

	[Deserializer.Skip]
	public GameFightCol Next { get; set; } = null;
};
