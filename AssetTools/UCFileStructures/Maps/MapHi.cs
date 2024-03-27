using Godot;
using System.Collections.Generic;

namespace AssetTools.UCFileStructures.Maps;

/// <summary>
/// The High Resolution Map
/// 
/// Originally called "PapHi", but according to developer notes, "Pap" is actually a new version of "Map".
/// </summary>
[Deserializer.DeserializeGenerator]
public partial class MapHi
{
	[Deserializer.Nested]
	public CompressedTextureInfo Texture { get; set; }

	/// <summary>
	/// Deserialized into Flags[]
	/// full but some sewer stuff that could go perhaps
	/// </summary>
	private ushort FlagsMask { get; set; }

	/// <summary>Extra flags about how this map cell should behave</summary>
	[Deserializer.Skip]
	public MapFlag[] Flags { get; set; }

	/// <summary>Compressed storage of Alt (expanded post deserialization into Altitude)</summary>
	public sbyte CompressedAltitude { get; set; }

	[Deserializer.Skip]
	public int Altitude { get; set; }

	/// <summary>
	/// padding; better find something to do with this 16K
	/// </summary>
	public sbyte Height { get; set; }

	/// <summary>
	/// Checks if "val" contains "flag", and if it does, adds "flag" to "flags".
	/// Returns val without "flag" set
	/// </summary>
	/// <param name="val"></param>
	/// <param name="flag"></param>
	/// <param name="flags"></param>
	/// <returns></returns>
	private ushort CheckAddFlag(ushort val, MapFlag flag, List<MapFlag> flags) {
		if (flag.IsSet(val)) {
			flags.Add(flag);
			val -= flag.Value;
		}

		return val;
	}

	partial void PostDeserialize() {
		this.Altitude = this.CompressedAltitude << 3;

		var flags = new List<MapFlag>();

		var val = this.FlagsMask;
		val = this.CheckAddFlag(val, MapFlag.Shadow1, flags);
		val = this.CheckAddFlag(val, MapFlag.Shadow2, flags);
		val = this.CheckAddFlag(val, MapFlag.Shadow3, flags);
		val = this.CheckAddFlag(val, MapFlag.Reflective, flags);
		val = this.CheckAddFlag(val, MapFlag.Hidden, flags);
		val = this.CheckAddFlag(val, MapFlag.SinkSquare, flags);
		val = this.CheckAddFlag(val, MapFlag.SinkPoint, flags);
		val = this.CheckAddFlag(val, MapFlag.NoUpper, flags);
		val = this.CheckAddFlag(val, MapFlag.NoGo, flags);
		val = this.CheckAddFlag(val, MapFlag.AnimTMap, flags);
		val = this.CheckAddFlag(val, MapFlag.RoofExists, flags);
		val = this.CheckAddFlag(val, MapFlag.Zone1, flags);
		val = this.CheckAddFlag(val, MapFlag.Zone2, flags);
		val = this.CheckAddFlag(val, MapFlag.Zone3, flags);
		val = this.CheckAddFlag(val, MapFlag.Zone4, flags);
		val = this.CheckAddFlag(val, MapFlag.Wander, flags);
		val = this.CheckAddFlag(val, MapFlag.FlatRoof, flags);
		val = this.CheckAddFlag(val, MapFlag.Water, flags);

		if (val > 0) {
			GD.PushError($"Reading flags did not zero it. Remaning value: {val}");
		}

		this.Flags = [.. flags];
	}
}
