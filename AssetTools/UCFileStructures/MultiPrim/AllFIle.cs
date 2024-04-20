using System;

namespace AssetTools.UCFileStructures.MultiPrim;

[Deserializer.DeserializeGenerator]
public partial class AllFile
{
	public int SaveType { get; set; }

	[Deserializer.Condition(When = "value.SaveType > 2")]
	public int Count { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(Count))]
	[Deserializer.Condition(When = "value.SaveType > 2")]
	[Deserializer.Nested]
	public MultiObject[] MultiObjects { get; set; }

	[Deserializer.Condition(When = "value.SaveType > 1")]
	[Deserializer.Nested]
	public GameChunk GameChunk { get; set; }

	public AllFile() {
		this.MultiObjects = [];
	}

	partial void PostDeserialize() {
		if (this.SaveType <= 2) {
			throw new Exception($"Invalid .all SaveType \"{this.SaveType}\". Expected > 2.");
		}
	}
}
