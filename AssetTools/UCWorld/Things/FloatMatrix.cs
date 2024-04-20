using AssetTools.UCFileStructures.MultiPrim;
using Godot;
using System;

namespace AssetTools.UCWorld.Things;

public class FloatMatrix
{
	/// <summary>
	/// [3][3]
	/// </summary>
	public float[][] M;

	public FloatMatrix(CMatrix33 cMatrix33) {
		const int CMAT0_MASK = 0x3ff00000;
		const int CMAT1_MASK = 0x000ffc00;
		const int CMAT2_MASK = 0x000003ff;

		this.M = [
			[
				((cMatrix33.M[0] & CMAT0_MASK) >> 20) / 512f,
				((cMatrix33.M[0] & CMAT1_MASK) >> 10) / 512f,
				((cMatrix33.M[0] & CMAT2_MASK) >> 00) / 512f
			],
			[
				((cMatrix33.M[1] & CMAT0_MASK) >> 20) / 512f,
				((cMatrix33.M[1] & CMAT1_MASK) >> 10) / 512f,
				((cMatrix33.M[1] & CMAT2_MASK) >> 00) / 512f
			],
			[
				((cMatrix33.M[2] & CMAT0_MASK) >> 20) / 512f,
				((cMatrix33.M[2] & CMAT1_MASK) >> 10) / 512f,
				((cMatrix33.M[2] & CMAT2_MASK) >> 00) / 512f
			],
		];

		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				if (this.M[i][j] >= 1f) this.M[i][j] -= 2f;
			}
		}
	}

	public Quaternion ToQuaternion() {
		int[] nxt = [1, 2, 0];
		float tr = this.M[0][0] + this.M[1][1] + this.M[2][2];

		// check the diagonal
		if (tr > 0f) {
			Quaternion quat = new Quaternion();
			float s = (float)Math.Sqrt(tr + 1f);
			quat.W = s / 2f;

			s = 0.5f / s;
			quat.X = (this.M[1][2] - this.M[2][1]) * s;
			quat.Y = (this.M[2][0] - this.M[0][2]) * s;
			quat.Z = (this.M[0][1] - this.M[1][0]) * s;

			return quat;
		} else {
			// diagonal is negative
			int i = 0;
			if (this.M[1][1] > this.M[0][0]) {
				i = 1;
			}
			if (this.M[2][2] > this.M[i][i]) {
				i = 2;
			}

			int j = nxt[i];
			int k = nxt[j];

			float s = (float)Math.Sqrt(this.M[i][i] - (this.M[j][j] + this.M[k][k]) + 1f);
			Quaternion quat = new Quaternion();
			float[] q = [0, 0, 0, 0];

			q[i] = s * 0.5f;

			if (s != 0f) {
				s = 0.5f / s;
			}

			q[3] = (this.M[j][k] - this.M[k][j]) * s;
			q[j] = (this.M[i][j] + this.M[j][i]) * s;
			q[k] = (this.M[i][k] + this.M[k][i]) * s;

			quat.X = q[0];
			quat.Y = q[1];
			quat.Z = q[2];
			quat.W = q[3];

			return quat;
		}
	}
}
