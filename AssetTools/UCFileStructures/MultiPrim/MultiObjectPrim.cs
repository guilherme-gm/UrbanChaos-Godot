using AssetTools.UCFileStructures.Prim;

namespace AssetTools.UCFileStructures.MultiPrim;

[Deserializer.DeserializeGenerator(AdditionalParams = ["int saveType"])]
public partial class MultiObjectPrim
{
	[Deserializer.FixedLenString(Length = 32)]
	public string Name;

	public int StartPoints;

	public int EndPoints;

	[Deserializer.Skip]
	public int PointsCount => this.EndPoints - this.StartPoints;

	[Deserializer.Condition(When = "saveType <= 3")]
	[Deserializer.VariableSizedArray(SizePropertyName = $"{nameof(PointsCount)}")]
	[Deserializer.Nested]
	private OldPrimPoint[] OldPrimPoints;

	[Deserializer.Condition(When = "saveType > 3")]
	[Deserializer.VariableSizedArray(SizePropertyName = $"{nameof(PointsCount)}")]
	[Deserializer.Nested]
	public PrimPoint[] PrimPoints;

	public int StartFace3;

	public int EndFace3;

	[Deserializer.Skip]
	public int Faces3Count => this.EndFace3 - this.StartFace3;

	[Deserializer.VariableSizedArray(SizePropertyName = $"{nameof(Faces3Count)}")]
	[Deserializer.Nested]
	public PrimFace3[] PrimFace3s { get; set; }

	public int StartFace4;

	public int EndFace4;

	[Deserializer.Skip]
	public int Faces4Count => this.EndFace4 - this.StartFace4;

	[Deserializer.VariableSizedArray(SizePropertyName = $"{nameof(Faces4Count)}")]
	[Deserializer.Nested]
	public PrimFace4[] PrimFace4s { get; set; }

	partial void PostDeserialize() {
		if (this.OldPrimPoints != null) {
			var pointsCount = this.PointsCount;
			this.PrimPoints = new PrimPoint[pointsCount];
			for (int i = 0; i < pointsCount; i++) {
				var oldPoint = this.OldPrimPoints[i];
				this.PrimPoints[i] = new PrimPoint() { X = (short)oldPoint.X, Y = (short)oldPoint.Y, Z = (short)oldPoint.Z };
			}

			this.OldPrimPoints = null;
		}

		foreach (var face3 in this.PrimFace3s) {
			face3.FixPointIds((ushort)this.StartPoints);
			face3.FixData(this.PrimPoints);
		}

		foreach (var face in this.PrimFace4s) {
			face.FixPointIds((ushort)this.StartPoints);
			face.FixData(this.PrimPoints);
			face.FaceFlags = (ushort)(face.FaceFlags & ~(1 << 6)); // FACE_FLAG_WALKABLE
		}
	}
}
