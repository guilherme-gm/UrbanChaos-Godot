namespace AssetTools.UCFileStructures.Maps.SuperMap;

[Deserializer.DeserializeGenerator]
public partial class StairCase
{
	/** // pos of staircase */
	public byte X { get; set; }
	public byte Z { get; set; }
	/** // flags for direction + up or down or both */
	public byte Flags { get; set; }
	/** // padding */
	public byte ID { get; set; }
	/** // link to next stair structure */
	public short NextStairs { get; set; }
	/** // link to next insidestorey for going downstairs */
	public short DownInside { get; set; }
	/** // link to next InsideStorey for going upstairs */
	public short UpInside { get; set; }
}
