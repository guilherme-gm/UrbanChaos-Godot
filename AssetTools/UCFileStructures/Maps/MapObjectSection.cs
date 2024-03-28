using AssetTools.UCFileStructures.Map;

namespace AssetTools.UCFileStructures.Maps;

[Deserializer.DeserializeGenerator]
public partial class MapObjectSection
{
	[Deserializer.Skip]
	private const int OB_SIZE = 32;

	public int ObjectCount { get; set; }

	/** The objects */
	[Deserializer.VariableSizedArray(SizePropertyName = nameof(ObjectCount))]
	[Deserializer.Nested]
	public MapObject[] Objects { get; set; }

	/** index (11 bits) / num (5 bits) */
	[Deserializer.FixedArray(Dimensions = [32 * 32])]
	private ushort[] FileMapwho { get; set; }

	[Deserializer.Skip]
	public Mapwho[][] Mapwho { get; set; }

	partial void PostDeserialize() {
		this.Mapwho = new Mapwho[OB_SIZE][];
		for (var i = 0; i < OB_SIZE; i++) {
			this.Mapwho[i] = new Mapwho[OB_SIZE];
		}

		for (var i = 0; i < this.FileMapwho.Length; i++) {
			int x = i / OB_SIZE;
			int z = i % OB_SIZE;
			this.Mapwho[x][z] = new Mapwho() {
				Index = this.FileMapwho[i] & ((1 << 11) - 1),
				Num = this.FileMapwho[i] >> 11,
			};
		}
	}
}
