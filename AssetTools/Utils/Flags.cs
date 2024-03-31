using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetTools.Utils;

/// <summary>
/// Represents a Flags container containing flags of type T.
/// This is a base structure to implement bit flags in a simpler way.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="Value">The full value</param>
public record struct Flags<T>(uint Value, T[] FlagsList) where T : IFlagsRecord
{
	public static Flags<T> FromNumber(uint number, T[] flagsList) {
		var flagsToSet = new List<T>();

		foreach (var flag in flagsList) {
			if ((number & flag.Value) == flag.Value) {
				flagsToSet.Add(flag);
				number -= flag.Value;
			}
		}

		if (number > 0) {
			throw new InvalidCastException($"Not all flags were consumed. Remaining value: {number}");
		}

		var flags = new Flags<T>(number, flagsList);
		flags.Set([.. flagsToSet]);

		return flags;
	}

	/// <summary>
	/// Sets a flag
	/// </summary>
	/// <param name="flag"></param>
	public void Set(T flag) {
		this.Value |= flag.Value;
	}

	/// <summary>
	/// Sets several flags
	/// </summary>
	/// <param name="flags"></param>
	public void Set(T[] flags) {
		foreach (var flag in flags) {
			this.Value |= flag.Value;
		}
	}

	/// <summary>
	/// Checks whether <paramref name="flag"/> is set
	/// </summary>
	/// <param name="flag"></param>
	/// <returns></returns>
	public readonly bool IsSet(T flag) {
		return (this.Value & flag.Value) == flag.Value;
	}

	/// <summary>
	/// Returns true if all <paramref name="flags"/> are set.
	/// </summary>
	/// <param name="flags"></param>
	/// <returns></returns>
	public readonly bool IsAllSet(T[] flags) {
		uint expectedMask = 0;
		foreach (var flag in flags) {
			expectedMask |= flag.Value;
		}

		return (this.Value & expectedMask) == expectedMask;
	}

	/// <summary>
	/// Returns true if at least one value in <paramref name="flags"/> is set
	/// </summary>
	/// <param name="flags"></param>
	/// <returns></returns>
	public readonly bool IsAnySet(T[] flags) {
		uint expectedMask = 0;
		foreach (var flag in flags) {
			expectedMask |= flag.Value;
		}

		return (this.Value & expectedMask) != 0;
	}

	public override readonly string ToString() {
		var this_ = this;
		var flagList = this.FlagsList
			.Where(this_.IsSet)
			.Select((val) => $"{val.Name} ({val.Value})")
			.ToArray()
			.Join(", ");

		return $"[{flagList}]";
	}
}
