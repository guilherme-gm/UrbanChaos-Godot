using Godot;

namespace AssetTools.Editor;

[Tool]
public static class Constants
{
	public static class Scenes
	{
		public static readonly StringName AssetToolsMenu = "res://Editor/AssetToolsMenu.tscn";

		public static readonly StringName PathConfigModal = "res://Editor/PathConfigModal.tscn";
	}

	public static class Config
	{
		public static readonly StringName Section = "uc_asset_tools";

		public static readonly StringName UCPathKey = "uc_path";

		public static readonly StringName WorkPathKey = "work_path";

	}
}
