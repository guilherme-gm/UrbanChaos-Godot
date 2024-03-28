using AssetTools.UCFileStructures.Maps;
using AssetTools.UCFileStructures.Maps.SuperMap;
using Godot;
using System;
using System.Collections.Generic;
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
	public SuperMapSection SuperMap => this.IamFile.SuperMap;
	public int TextureSet => this.IamFile.TextureSet;
	public byte[] PsxTexturesXY => this.SaveType >= 25 ? this.IamFile.PsxTexturesXY : null;

	public string OBSizeString => this.SaveType > 23 ? this.OBSize.ToString() : "N/A";
	public string TextureSetString => this.SaveType >= 20 ? this.TextureSet.ToString() : "N/A";

	public FloorStore[] FloorStores => this.HighResMap.Select(FloorStore.FromMapHi).ToArray();

	public List<MapObject> MapObjects {
		get {
			var list = new List<MapObject>();

			for (var x = 0; x < this.IamFile.MapObjects.Mapwho.Length; x++) {
				for (var z = 0; z < this.IamFile.MapObjects.Mapwho[x].Length; z++) {
					var mapWho = this.IamFile.MapObjects.Mapwho[x][z];
					var index = mapWho.Index;
					var num = mapWho.Num;

					while (num > 0) {
						num--;
						if (index < 0 || index > this.IamFile.MapObjects.Objects.Length) {
							throw new Exception($"Could not read object index {index}. Out of range");
						}

						var ob = this.IamFile.MapObjects.Objects[index];
						if (ob.Prim != 0) {
							// @TODO: handle Damaged flag
							var of = new MapObject() {
								Prim = ob.Prim,
								Position = new Vector3(
									((x << 10) + ob.LocalX) / 256,
									ob.Y,
									((z << 10) + ob.LocalZ) / 256
								),
								Crumple = 0,
								Flags = ob.Flags,
								Index = index,
								InsideIndex = ob.InsideIndex,
								Pitch = 0,
								Roll = 0,
								Yaw = ob.Yaw
							};
							list.Add(of);
						} else {
							GD.PushWarning($"Zeroed prim");
						}

						index++;
					}
				}
			}

			return list;
		}
	}
}
