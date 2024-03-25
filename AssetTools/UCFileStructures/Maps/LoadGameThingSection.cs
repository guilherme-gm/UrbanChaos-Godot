namespace AssetTools.UCFileStructures.Maps;

[Deserializer.DeserializeGenerator]
public partial class LoadGameThingSection
{
	public ushort Count { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(Count))]
	[Deserializer.Nested]
	public LoadGameThing[] LoadGameThings { get; set; }
}
