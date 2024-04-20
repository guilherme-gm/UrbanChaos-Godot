using Godot;
using System;

namespace AssetTools.UCFileStructures.MultiPrim;

[Deserializer.DeserializeGenerator]
public partial class GameChunk
{
	public int SaveType { get; set; }
	public int ElementCount { get; set; }
	public short MaxPeopleType { get; set; }
	public short MaxAnimFrames { get; set; }
	public int MaxElements { get; set; }
	public short MaxKeyFrames { get; set; }
	public short MaxFightCols { get; set; }


	#region Load the people types

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(MaxPeopleType))]
	[Deserializer.Nested]
	public BodyDef[] PeopleTypes { get; set; }

	// Must be == 666
	public ushort SanityCheck1 { get; set; }

	#endregion


	#region Load the keyframe linked lists

	public uint Addr1 { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(MaxKeyFrames))]
	[Deserializer.Nested]
	public GameKeyFrame[] AnimKeyFrames { get; set; }

	// Must be == 666
	public ushort SanityCheck2 { get; set; }

	#endregion


	#region Load the anim elements

	public uint Addr2 { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(MaxElements))]
	[Deserializer.Nested]
	public GameKeyFrameElement[] TheElements { get; set; }

	// Must be == 666
	public ushort SanityCheck3 { get; set; }

	#endregion

	#region Load the animlist


	// Original code does some extrapolation with the pointer being an inde to another array, here we have 2 properties
	[Deserializer.VariableSizedArray(SizePropertyName = nameof(MaxAnimFrames))]
	public uint[] AnimListIdx { get; set; }

	[Deserializer.Skip]
	public GameKeyFrame[] AnimList { get; set; }

	// Must be == 666
	public ushort SanityCheck4 { get; set; }

	public uint Addr3 { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(MaxFightCols))]
	[Deserializer.Nested]
	public GameFightCol[] FightCols { get; set; }

	// Must be == 666
	public ushort SanityCheck5 { get; set; }

	#endregion

	partial void PostDeserialize() {
		if (this.SanityCheck1 != 666) {
			throw new Exception($"Invalid SanityCheck1 \"{this.SanityCheck1}\". Expected 666.");
		}
		if (this.SanityCheck2 != 666) {
			throw new Exception($"Invalid SanityCheck2 \"{this.SanityCheck2}\". Expected 666.");
		}
		if (this.SanityCheck3 != 666) {
			throw new Exception($"Invalid SanityCheck3 \"{this.SanityCheck3}\". Expected 666.");
		}
		if (this.SanityCheck4 != 666) {
			throw new Exception($"Invalid SanityCheck4 \"{this.SanityCheck4}\". Expected 666.");
		}
		if (this.SanityCheck5 != 666) {
			throw new Exception($"Invalid SanityCheck5 \"{this.SanityCheck5}\". Expected 666.");
		}

		if (this.SaveType < 3) {
			this.ConvertAnimSpeeds();
		}

		if (this.SaveType > 4) {
			this.ConvertKeyframeToPointer();
			this.ConvertAnimlistToPointer();
			this.ConvertFightColToPointer();
		} else {
			this.DoPsxConversion();
		}
	}

	/// <summary>
	/// I am not sure if this is a good name... there was a comment saying "PSX1 game chunk" at this code.
	/// </summary>
	private void DoPsxConversion() {
		GD.PushWarning("@TODO: PSX conversion");
	}

	private void ConvertKeyframeToPointer() {
		for (int i = 0; i < this.MaxKeyFrames; i++) {
			var firstElementIdx = this.AnimKeyFrames[i].FirstElementIdx;
			if (firstElementIdx < 0) {
				this.AnimKeyFrames[i].FirstElement = null;
			} else {
				this.AnimKeyFrames[i].FirstElement = this.TheElements[firstElementIdx];
			}

			var prevFrameIdx = this.AnimKeyFrames[i].PrevFrameIdx;
			if (prevFrameIdx < 0) {
				this.AnimKeyFrames[i].PrevFrame = null;
			} else {
				this.AnimKeyFrames[i].PrevFrame = this.AnimKeyFrames[prevFrameIdx];
			}

			var nextFrameIdx = this.AnimKeyFrames[i].NextFrameIdx;
			if (nextFrameIdx < 0) {
				this.AnimKeyFrames[i].NextFrame = null;
			} else {
				this.AnimKeyFrames[i].NextFrame = this.AnimKeyFrames[nextFrameIdx];
			}

			var fightIdx = this.AnimKeyFrames[i].FightIdx;
			if (fightIdx < 0) {
				this.AnimKeyFrames[i].Fight = null;
			} else {
				this.AnimKeyFrames[i].Fight = this.FightCols[fightIdx];
			}
		}
	}

	private void ConvertAnimlistToPointer() {
		// @FIXME: Something is not mapping right here...
		this.AnimList = new GameKeyFrame[this.MaxAnimFrames];
		int skip = 0;
		for (int i = 0; i < this.MaxAnimFrames; i++) {
			var idx = this.AnimListIdx[i];
			if (idx < 0 || idx > this.AnimKeyFrames.Length) {
				skip++;
				continue;
			}
			this.AnimList[i] = this.AnimKeyFrames[idx];
		}

		if (skip > 0) {
			GD.PushWarning($">> ConvertAnimlistToPointer: {skip} of {this.MaxAnimFrames} skipped");
		}
	}

	private void ConvertFightColToPointer() {
		for (int i = 0; i < this.MaxFightCols; i++) {
			if (this.FightCols[i].NextIdx < 0) {
				this.FightCols[i].Next = null;
			} else {
				this.FightCols[i].Next = this.FightCols[this.FightCols[i].NextIdx];
			}
		}
	}

	/// <summary>
	/// convert anim speeds to step sizes rather than step counts
	/// </summary>
	private void ConvertAnimSpeeds() {
		for (int i = 0; i < this.MaxKeyFrames; i++) {
			int part1 = 256 / (this.AnimKeyFrames[i].TweenStep + 1);
			this.AnimKeyFrames[i].TweenStep = (byte)(part1 / 2);
		}
	}
}
