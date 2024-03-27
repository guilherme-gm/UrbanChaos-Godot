using AssetTools.UCFileStructures.Maps;
using AssetTools.UCFileStructures.Maps.SuperMap;
using System.Linq;

namespace AssetTools.Structures;

public class Map
{
	public AssetLoadStatus IamStatus { get; set; } = AssetLoadStatus.NotLoaded;
	public string IamFilePath { get; set; } = "";
	public Iam IamFile { get; set; } = null;

	public int SaveType => this.IamFile.SaveType;
	public int OBSize => this.IamFile.OBSize;
	public MapHi[] HighResMap => this.IamFile.Map;
	public MapThingPsxSection MapThingPsxSection => this.IamFile.MapThingPsxSection;
	public LoadGameThingSection LoadGameThingSection => this.IamFile.LoadGameThingSection;
	public MapObjectSection MapObjects => this.SaveType < 23
		? this.IamFile.OldMapObjects
		: this.IamFile.SuperMap.SupermapMapObjects;
	public SuperMapSection SuperMap => this.IamFile.SuperMap;
	public int TextureSet => this.IamFile.TextureSet;
	public byte[] PsxTexturesXY => this.SaveType >= 25 ? this.IamFile.PsxTexturesXY : null;


	public string OBSizeString => this.SaveType > 23 ? this.OBSize.ToString() : "N/A";
	public string TextureSetString => this.SaveType >= 20 ? this.TextureSet.ToString() : "N/A";

	public FloorStore[] FloorStores => this.HighResMap.Select(FloorStore.FromMapHi).ToArray();
}
