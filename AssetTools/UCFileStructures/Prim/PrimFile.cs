namespace AssetTools.UCFileStructures.Prim;

/**
 * For older versions of the prim files (prim<%d>.prm)
 */
[Deserializer.DeserializeGenerator]
public partial class PrimFile : IPrim
{
	// public short SaveType { get; set; }
	[Deserializer.FixedLenString(Length = 32)]
	public string Name { get; set; }

	[Deserializer.Nested]
	public NPrim Prim { get; set; }

	// Originally, Dummy4 inside OldPrim, but that's the only difference between Prim and OldPrim
	// Also, Dummy[3] is the SaveType
	[Deserializer.FixedArray(Dimensions = [3])]
	public short[] Dummy { get; set; }

	// Originally part of Prim.Dummy[3] , but this is exactly the last item...
	public short SaveType { get; set; }

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

	public PrimFile() {
		this.PrimPoints = [];
		this.PrimFace3s = [];
		this.PrimFace4s = [];
	}

#pragma warning disable IDE0051 // Remove unused private members -- This is a hook from the CodeGenerator
	/**
	 * Perform some structural fixes after loading
	 */
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
		}

		foreach (var face4 in this.PrimFace4s) {
			face4.FixPointIds(this.Prim.PointsStartId);
		}
	}
#pragma warning restore IDE0051 // Remove unused private members
}
