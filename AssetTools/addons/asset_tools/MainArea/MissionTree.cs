using AssetTools.Structures;
using Godot;
using System;

namespace AssetTools.Addons.Asset_Tools;

[Tool]
public partial class MissionTree : Tree
{
	private Mission Mission = null;

	public override void _Ready() {
		this.SetColumnTitle(0, "Name");
		this.SetColumnTitle(1, "Value");
	}

	public void SetMission(Mission mission) {
		this.Mission = mission;
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

#if RENDER_HIGHRES_ITEMS
		for (int i = 0; i < highResMap.Length; i++) {
			var item = highResMap[i];

			_ = this.CreateItem(
				mapHiNode,
				$"Item {i}",
				$"Texture = {item.Texture} ; Flags = {item.Flags} ; Alt = {item.Alt} ; Height = {item.Height}"
			);
			/*
			_ = this.CreateItem(itemNode, "Texture", item.Texture.ToString());
			_ = this.CreateItem(itemNode, "Flags", item.Flags.ToString());
			_ = this.CreateItem(itemNode, "Alt", item.Alt.ToString());
			_ = this.CreateItem(itemNode, "Height", item.Height.ToString());
			*/
		}
#endif
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

	private void DrawMapFileNode(TreeItem missionNode) {
		var mapNode = this.CreateItem(missionNode, "Map file (IAM)", this.Mission.Map.IamFilePath, this.Mission.Map.IamStatus);
		if (this.Mission.Map.IamStatus != AssetLoadStatus.Loaded) {
			return;
		}

		_ = this.CreateItem(mapNode, "SaveType", this.Mission.Map.SaveType.ToString());
		_ = this.CreateItem(mapNode, "TextureSet", this.Mission.Map.TextureSetString);
		_ = this.CreateItem(mapNode, "OBSize", this.Mission.Map.OBSizeString);
		this.DrawHighResMapNode(mapNode);
		_ = this.CreateItem(mapNode, "MapThingPsxSection", this.Mission.Map.MapThingPsxSection?.ToString() ?? "N/A");
		this.DrawLoadGameThing(mapNode);
		this.DrawMapObjects(mapNode);
		_ = this.CreateItem(mapNode, "SuperMap", this.Mission.Map.SuperMap.ToString());
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
