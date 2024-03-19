using Godot;

namespace AssetTools.UCFileStructures.Prim;

/**
 * For older versions of the prim files (nprim<%d>.prm)
 */
[Deserializer.DeserializeGenerator]
public partial class NPrimFile : IPrim
{
	/**
	 * PRIM_START_SAVE_TYPE	5793
	 * PRIM_END_SAVE_TYPE		5800
	 */
	public short SaveType { get; set; }

	[Deserializer.FixedLenString(Length = 32)]
	public string Name { get; set; }

	[Deserializer.Nested]
	public NPrim Prim { get; set; }

	[Deserializer.Condition(When = "value.SaveType == 5794")]
	[Deserializer.VariableSizedArray(SizePropertyName = $"{nameof(Prim)}.{nameof(Prim.PointsCount)}")]
	[Deserializer.Nested]
	public PrimPoint[] PrimPoints { get; set; }

	[Deserializer.Condition(When = "value.SaveType != 5794")]
	[Deserializer.VariableSizedArray(SizePropertyName = $"{nameof(Prim)}.{nameof(Prim.PointsCount)}")]
	[Deserializer.Nested]
	private OldPrimPoint[] OldPrimPoints { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = $"{nameof(Prim)}.{nameof(Prim.Faces3Count)}")]
	[Deserializer.Nested]
	public PrimFace3[] PrimFace3s { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = $"{nameof(Prim)}.{nameof(Prim.Faces4Count)}")]
	[Deserializer.Nested]
	public PrimFace4[] PrimFace4s { get; set; }

	public NPrimFile() {
		this.PrimPoints = [];
		this.PrimFace3s = [];
		this.PrimFace4s = [];
	}

	/**
	 * Perform some structural fixes after loading
	 */
#pragma warning disable IDE0051 // Remove unused private members
	partial void PostDeserialize() {
		if (this.OldPrimPoints != null) {
			var pointsCount = this.Prim.PointsCount;
			this.PrimPoints = new PrimPoint[pointsCount];
			for (int i = 0; i < pointsCount; i++) {
				var oldPoint = this.OldPrimPoints[i];
				this.PrimPoints[i] = new PrimPoint() { X = (short)oldPoint.X, Y = (short)oldPoint.Y, Z = (short)oldPoint.Z };
			}

			this.OldPrimPoints = null;
		}

		foreach (var face3 in this.PrimFace3s) {
			face3.FixPointIds(this.Prim.PointsStartId);
			face3.FixData(this.PrimPoints);
		}

		foreach (var face4 in this.PrimFace4s) {
			face4.FixPointIds(this.Prim.PointsStartId);
			face4.FixData(this.PrimPoints);
		}
	}
#pragma warning restore IDE0051 // Remove unused private members
}
