namespace AssetTools.UCFileStructures.Maps.SuperMap;

[Deserializer.DeserializeGenerator]
public partial class DStorey
{
	/**  //replacement style           // maybe this could be a byte */
	public ushort Style { get; set; }
	/** //Index to painted info */
	public ushort Index { get; set; }
	/** //+ve is a style  //-ve is a  //get rid of this */
	public sbyte Count { get; set; }
	public byte BloodyPadding { get; set; }
}
