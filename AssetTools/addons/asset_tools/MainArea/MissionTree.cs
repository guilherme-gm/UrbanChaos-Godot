using AssetTools.Structures;
using AssetTools.UCFileStructures.Maps.SuperMap;
using Godot;
using System;
using System.Linq;

namespace AssetTools.Addons.Asset_Tools;

[Tool]
public partial class MissionTree : Tree
{
	private Mission Mission = null;

	private bool DrawHighResNodes { get; set; } = false;

	public override void _Ready() {
		this.SetColumnTitle(0, "Name");
		this.SetColumnTitle(1, "Value");
	}

	public void SetMission(Mission mission, bool drawHighResNodes) {
		this.Mission = mission;
		this.DrawHighResNodes = drawHighResNodes;
		this.RedrawTree();
	}

	private TreeItem CreateItem(TreeItem parent, string name, string value) {
		var item = this.CreateItem(parent);
		item.SetText(0, name);
		item.SetText(1, value);

		return item;
	}

	private TreeItem CreateItem(TreeItem parent, string name, string value, AssetLoadStatus status) {
		var item = this.CreateItem(parent);
		item.SetText(0, name);
		item.SetText(1, $"{value} ({status.Description})");
		item.SetCustomColor(1, Color.FromHtml(status.ColorHex));

		return item;
	}

	private string MaybeArraySizeText(Array arr) {
		var text = "N/A";
		if (arr?.Length > 0) {
			text = $"{arr.Length} items";
		}

		return text;
	}

	private void DrawHighResMapNode(TreeItem mapNode) {
		var highResMap = this.Mission.Map.HighResMap;

		var mapHiNode = this.CreateItem(mapNode, "High Res Map (MapHi)", this.MaybeArraySizeText(highResMap));
		if (highResMap?.Length == 0) {
			return;
		}

		mapHiNode.Collapsed = true;

		if (this.DrawHighResNodes) {
			for (int i = 0; i < highResMap.Length; i++) {
				var item = highResMap[i];

				_ = this.CreateItem(
					mapHiNode,
					$"Item ({i / 128}, {i % 128})",
					$"Texture = {item.Texture} ; Flags = [{String.Join(", ", item.Flags.Select(v => v.Name))}] ; Alt = {item.Alt} ; Height = {item.Height}"
				);
				/*
				_ = this.CreateItem(itemNode, "Texture", item.Texture.ToString());
				_ = this.CreateItem(itemNode, "Flags", item.Flags.ToString());
				_ = this.CreateItem(itemNode, "Alt", item.Alt.ToString());
				_ = this.CreateItem(itemNode, "Height", item.Height.ToString());
				*/
			}
		}
	}

	private void DrawFloorStore(TreeItem mapNode) {
		var floorStores = this.Mission.Map.FloorStores;

		var mapHiNode = this.CreateItem(mapNode, "Floor Stores (from MapHi)", this.MaybeArraySizeText(floorStores));
		if (floorStores?.Length == 0) {
			return;
		}

		mapHiNode.Collapsed = true;

		if (this.DrawHighResNodes) {
			for (int i = 0; i < floorStores.Length; i++) {
				var item = floorStores[i];

				var texture = item.TexturePage == 309 ? "309 (General Steam)" : item.TexturePage.ToString();

				_ = this.CreateItem(
					mapHiNode,
					$"Item ({i / 128}, {i % 128})",
					$"TexturePage = {texture} ; Flags = [{String.Join(", ", item.Flags.Select(v => v.Name))}] ; Alt = {item.Alt} ; X = {item.X} ; Z = {item.Z}"
				);
				/*
				_ = this.CreateItem(itemNode, "Texture", item.Texture.ToString());
				_ = this.CreateItem(itemNode, "Flags", item.Flags.ToString());
				_ = this.CreateItem(itemNode, "Alt", item.Alt.ToString());
				_ = this.CreateItem(itemNode, "Height", item.Height.ToString());
				*/
			}
		}
	}

	private void DrawLoadGameThing(TreeItem mapNode) {
		var section = this.Mission.Map.LoadGameThingSection;
		if (section == null) {
			_ = this.CreateItem(mapNode, "Load Game Thing", "N/A");
			return;
		}

		var rootNode = this.CreateItem(mapNode, "Load Game Thing", $"{section.Count} items");
		rootNode.Collapsed = true;

		for (int i = 0; i < section.Count; i++) {
			var item = section.LoadGameThings[i];

			_ = this.CreateItem(
				rootNode,
				$"Item {i}",
				$"Type = {item.Type} ; SubStype = {item.SubStype} ; X = {item.X} ; Y = {item.Y} ; Z = {item.Z} ; Flags = {item.Flags} ; IndexOther = {item.IndexOther} ; AngleX = {item.AngleX} ; AngleY = {item.AngleY} ; AngleZ = {item.AngleZ}"
			);
		}
	}

	private void DrawMapObjects(TreeItem mapNode) {
		var section = this.Mission.Map.MapObjects;
		if (section == null) {
			_ = this.CreateItem(mapNode, "Map Objects", "N/A");
			return;
		}

		var rootNode = this.CreateItem(mapNode, "Map Objects", "");
		rootNode.Collapsed = true;

		var objectsRootNode = this.CreateItem(rootNode, "Objects", $"{section.ObjectCount} items");
		objectsRootNode.Collapsed = true;

		for (int i = 0; i < section.ObjectCount; i++) {
			var item = section.Objects[i];

			_ = this.CreateItem(
				objectsRootNode,
				$"Item {i}",
				$"X = {item.X} ; Y = {item.Y} ; Z = {item.Z} ; Prim = {item.Prim} ; Yaw = {item.Yaw} ; Flags = {item.Flags} ; InsideIndex = {item.InsideIndex}"
			);
		}

		var mapwhoNode = this.CreateItem(rootNode, "Mapwho", $"{section.Mapwho.Length} items");
		mapwhoNode.Collapsed = true;

		for (int i = 0; i < section.Mapwho.Length; i++) {
			var item = section.Mapwho[i];
			var index = item >> 5;
			var num = item & 011111;

			_ = this.CreateItem(
				mapwhoNode,
				$"Item {i}",
				$"Index = {index} ; Num = {num}"
			);
		}
	}

	private void DrawArrayNode<T>(TreeItem mapNode, string title, T[] entries, Func<T, string> valueFn) {
		var rootNode = this.CreateItem(mapNode, title, $"{entries.Length} items");
		rootNode.Collapsed = true;

		for (int i = 0; i < entries.Length; i++) {
			var item = entries[i];

			_ = this.CreateItem(
				rootNode,
				$"Item {i}",
				valueFn(item)
			);
		}
	}

	private void DrawMapFileNode(TreeItem missionNode) {
		var mapNode = this.CreateItem(missionNode, "Map file (IAM)", this.Mission.Map.IamFilePath, this.Mission.Map.IamStatus);
		if (this.Mission.Map.IamStatus != AssetLoadStatus.Loaded) {
			return;
		}

		_ = this.CreateItem(mapNode, "SaveType", this.Mission.Map.SaveType.ToString());
		_ = this.CreateItem(mapNode, "TextureSet", this.Mission.Map.TextureSetString);
		_ = this.CreateItem(mapNode, "OBSize", this.Mission.Map.OBSizeString);
		this.DrawHighResMapNode(mapNode);
		this.DrawFloorStore(mapNode);
		_ = this.CreateItem(mapNode, "MapThingPsxSection", this.Mission.Map.MapThingPsxSection?.ToString() ?? "N/A");
		this.DrawLoadGameThing(mapNode);
		this.DrawMapObjects(mapNode);
		this.DrawArrayNode(mapNode, "SuperMap/DBuildings", this.Mission.Map.IamFile.SuperMap.DBuildings,
			(DBuilding v) => $"X = {v.X} ; Y = {v.Y} ; Z = {v.Z} ; StartFacet = {v.StartFacet} ; EndFacet = {v.EndFacet} ; Walkable = {v.Walkable} ; Counter = [{v.Counter[0]}, {v.Counter[1]}] ; Padding = {v.Padding} ; Ware = {v.Ware} ; Type = {v.Type}");
		this.DrawArrayNode(mapNode, "SuperMap/DFacet", this.Mission.Map.IamFile.SuperMap.DFacets,
			(DFacet v) => $"FacetType = {v.FacetType} ; Height = {v.Height} ; X = [{v.X[0]}, {v.X[1]}] ; Y = [{v.Y[0]}, {v.Y[1]}] ; Z = [{v.Z[0]}, {v.Z[1]}] ; FacetFlags = {v.FacetFlags} ; StyleIndex = {v.StyleIndex} ; Building = {v.Building} ; DStorey = {v.DStorey} ; FHeight = {v.FHeight} ; BlockHeight = {v.BlockHeight} ; Open = {v.Open} ; Dfcache = {v.Dfcache} ; Shake = {v.Shake} ; CutHole = {v.CutHole} ; Counter = [{v.Counter[0]}, {v.Counter[1]}]");
		this.DrawArrayNode(mapNode, "SuperMap/DStyles", this.Mission.Map.IamFile.SuperMap.DStyles,
			(ushort v) => $"{v}");
		this.DrawArrayNode(mapNode, "SuperMap/PaintMem", this.Mission.Map.IamFile.SuperMap.PaintMem,
			(byte v) => $"{v}");
		this.DrawArrayNode(mapNode, "SuperMap/DStoreys", this.Mission.Map.IamFile.SuperMap.DStoreys,
			(DStorey v) => $"Style = {v.Style} ; Index = {v.Index} ; Count = {v.Count} ; BloodyPadding = {v.BloodyPadding}");
		_ = this.CreateItem(mapNode, "SuperMap/Insides", "@TODO");
		this.DrawArrayNode(mapNode, "SuperMap/DStoreys", this.Mission.Map.IamFile.SuperMap.DStoreys,
			(DStorey v) => $"Style = {v.Style} ; Index = {v.Index} ; Count = {v.Count} ; BloodyPadding = {v.BloodyPadding}");

		var walkablesSection = this.CreateItem(mapNode, "SuperMap/Walkables", "");
		this.DrawArrayNode(walkablesSection, "DWalkable", this.Mission.Map.IamFile.SuperMap.WalkablesSection.DWalkables,
			(DWalkable v) => $"StartPoint = {v.StartPoint} ; EndPoint = {v.EndPoint} ; StartFace3 = {v.StartFace3} ; EndFace3 = {v.EndFace3} ; StartFace4 = {v.StartFace4} ; EndFace4 = {v.EndFace4} ; X1 = {v.X1} ; Z1 = {v.Z1} ; X2 = {v.X2} ; Z2 = {v.Z2} ; Y = {v.Y} ; StoreyY = {v.StoreyY} ; Next = {v.Next} ; Building = {v.Building}");
		this.DrawArrayNode(walkablesSection, "RoofFace4s", this.Mission.Map.IamFile.SuperMap.WalkablesSection.RoofFace4s,
			(RoofFace4 v) => $"Y = {v.Y} ; DY = [{v.DY[0]}, {v.DY[1]}, {v.DY[2]}] ; DrawFlags = {v.DrawFlags} ; RX = {v.RX} ; RZ = {v.RZ} ; Next = {v.Next}");

		_ = this.CreateItem(mapNode, "PsxTexturesXY", this.MaybeArraySizeText(this.Mission.Map.PsxTexturesXY));
	}

	private void RedrawTree() {
		this.Clear();

		var missionNode = this.CreateItem(null, "Mission (UCM)", this.Mission.UcmFilePath, this.Mission.UcmStatus);
		_ = this.CreateItem(missionNode, "Version", this.Mission.Version.ToString());
		_ = this.CreateItem(missionNode, "Used", this.Mission.Used.ToString());
		_ = this.CreateItem(missionNode, "MissionName", this.Mission.MissionName);
		this.DrawMapFileNode(missionNode);
		_ = this.CreateItem(missionNode, "BriefName", this.Mission.BriefFileName);
		_ = this.CreateItem(missionNode, "LightMapName", this.Mission.LightMapFileName);
		_ = this.CreateItem(missionNode, "CitSezName", this.Mission.CitSezFileName);
	}
}
