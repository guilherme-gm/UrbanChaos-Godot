using Godot;
using System;

namespace AssetTools.UCFileStructures.Prim;

public abstract class PrimFace
{
	private const ushort FACE_FIXED = 1 << 5;
	private const ushort TEXTURE_NORM_SQUARES = 8;
	private const ushort TEXTURE_NORM_SIZE = 32;
	private const ushort FACE_FLAG_THUG_JACKET = 1 << 15;
	private const ushort FACE_PAGE_OFFSETS = 8 * 64;
	private const int FACE_FLAG_ANIMATE = 1 << 9;


	public Vector3 Normals { get; set; }

	public float[][] UV { get; set; }

	public int TexturePage { get; set; }

	// TEXTURE_fix_prim_textures
	private void FixFacesPart1() {
		var this_ = this as IPrimFace;

		if ((this_.FaceFlags & FACE_FIXED) != 0) {
			GD.Print($"Skipping face (1) -- flag: {this_.FaceFlags}");
			return;
		}

		if ((this_.FaceFlags & FACE_FLAG_ANIMATE) != 0) {
			GD.Print($"Skipping face (2) -- flag: {this_.FaceFlags}");
			return;
		}

		int av_u = 0;
		for (int i = 0; i < this_.CompressedUV.Length; i++)
			av_u += this_.CompressedUV[i][0];
		av_u /= this_.CompressedUV.Length;

		int av_v = 0;
		for (int i = 0; i < this_.CompressedUV.Length; i++)
			av_v += this_.CompressedUV[i][1];
		av_v /= this_.CompressedUV.Length;

		av_u /= TEXTURE_NORM_SIZE;
		av_v /= TEXTURE_NORM_SIZE;

		int base_u = av_u * TEXTURE_NORM_SIZE;
		int base_v = av_v * TEXTURE_NORM_SIZE;

		for (int i = 0; i < this_.CompressedUV.Length; i++) {
			int u = this_.CompressedUV[i][0];
			int v = this_.CompressedUV[i][1];

			u -= base_u;
			v -= base_v;

			u = Math.Clamp(u, 0, 32);
			v = Math.Clamp(v, 0, 32);

			if (u == 31)
				u = 32;

			if (v == 31)
				v = 32;

			this_.CompressedUV[i][0] = (byte)u;
			this_.CompressedUV[i][1] = (byte)v;
		}

		var page = av_u + (av_v * TEXTURE_NORM_SQUARES) + (this_.CompressedTexturePage * TEXTURE_NORM_SQUARES * TEXTURE_NORM_SQUARES);
		this_.FaceFlags = (ushort)(this_.FaceFlags & ~FACE_FLAG_THUG_JACKET);

		switch (page) {
			//
			// pages 9, 10 , 11 are people
			//
			case (9 * 64) + 21:
			case (18 * 64) + 2:
			case (18 * 64) + 32:
				//					ASSERT(0);
				this_.FaceFlags |= FACE_FLAG_THUG_JACKET;
				page = (9 * 64) + 21;

				break;

			case (9 * 64) + 22:
			case (18 * 64) + 3:
			case (18 * 64) + 33:
				this_.FaceFlags |= FACE_FLAG_THUG_JACKET;
				page = (9 * 64) + 22;
				break;

			case (9 * 64) + 24:
			case (18 * 64) + 4:
			case (18 * 64) + 36:
				this_.FaceFlags |= FACE_FLAG_THUG_JACKET;
				page = (9 * 64) + 24;
				break;

			case (9 * 64) + 25:
			case (18 * 64) + 5:
			case (18 * 64) + 37:
				this_.FaceFlags |= FACE_FLAG_THUG_JACKET;
				page = (9 * 64) + 25;
				break;

			default:
				break;
		}

		page -= FACE_PAGE_OFFSETS;
		page = Math.Clamp(page, 0, 64 * 14);

		this_.CompressedUV[0][0] |= (byte)((page >> 2) & 0xc0);
		this_.CompressedTexturePage = (byte)((page >> 0) & 0xff);
		this_.FaceFlags |= FACE_FIXED;
	}

	// This is inside _Draw functions
	private void FixFacesPart2() {
		var this_ = this as IPrimFace;
		this.UV = new float[this_.CompressedUV.Length][];

		this.UV[0] = [
			(this_.CompressedUV[0][0] & 0x3f) * (1f / 32f),
			this_.CompressedUV[0][1] * (1f / 32f),
		];

		for (int i = 1; i < this_.CompressedUV.Length; i++) {
			this.UV[i] = [
				this_.CompressedUV[i][0] * (1f / 32f),
				this_.CompressedUV[i][1] * (1f / 32f),
			];
		}

		var page = this_.CompressedUV[0][0] & 0xc0;
		page <<= 2;
		page |= this_.CompressedTexturePage;
		page += 8 * 64; // FACE_PAGE_OFFSET

		this.TexturePage = page;
	}

	// calc_prim_normals
	private void CalcNormal(PrimPoint[] points) {
		var this_ = this as IPrimFace;

		int numSides = this_.Points.Length;
		PrimPoint p_op0, p_op1, p_op2;
		Vector3 p_normal = new Vector3();

		if (numSides == 3) // tri's
		{
			// this_face3 = &prim_faces3[-face];
			p_op0 = points[this_.Points[0]]; // &prim_points[this_face3->Points[0]];
			p_op1 = points[this_.Points[1]]; // &prim_points[this_face3->Points[1]];
			p_op2 = points[this_.Points[2]]; // &prim_points[this_face3->Points[2]];
		} else {
			// this_face4 = &prim_faces4[face];
			p_op0 = points[this_.Points[0]]; // &prim_points[this_face4->Points[0]];
			p_op1 = points[this_.Points[1]]; // &prim_points[this_face4->Points[1]];
			p_op2 = points[this_.Points[3]]; // &prim_points[this_face4->Points[3]];
		}

		int vx = -p_op0.X + p_op1.X;
		int vy = -p_op0.Y + p_op1.Y; // vector from point 0 to point 1
		int vz = -p_op0.Z + p_op1.Z;

		int wx = p_op2.X - p_op1.X; // vector from point 1 to point 2
		int wy = p_op2.Y - p_op1.Y;
		int wz = p_op2.Z - p_op1.Z;

		if ((vx == 0 && vy == 0 && vz == 0) || (wx == 0 && wy == 0 && wz == 0)) {
			p_normal.X = 0;
			p_normal.Y = 255; // store result
			p_normal.Z = 0;
			this.Normals = p_normal;
			return;
		}

		int length = (vx * vx) + (vy * vy) + (vz * vz);
		length = (int)Math.Sqrt(length);
		if (length == 0)
			length = 1; // bodge around divide by zero
		vx = (vx << 8) / length;
		vy = (vy << 8) / length; // normalise vect V ( make length equal 1<<8)
		vz = (vz << 8) / length;

		length = (int)Math.Sqrt((wx * wx) + (wy * wy) + (wz * wz));
		if (length == 0)
			length = 1; // bodge around divide by zero

		wx = (wx << 8) / length;
		wy = (wy << 8) / length; // same to vect W
		wz = (wz << 8) / length;

		int nx = ((vy * wz) - (vz * wy)) >> 8;
		int ny = ((vz * wx) - (vx * wz)) >> 8; // perform cross product on vect V & W
		int nz = ((vx * wy) - (vy * wx)) >> 8;

		length = (int)Math.Sqrt((nx * nx) + (ny * ny) + (nz * nz));
		if (length == 0)
			length = 1;
		nx = (nx << 8) / length;
		ny = (ny << 8) / length; // normalise result  //pos opt this out
		nz = (nz << 8) / length;
		if (nx == 0 && ny == 0 && nz == 0)
			ny = 255;
		p_normal.X = -nx;
		p_normal.Y = -ny; // store result
		p_normal.Z = -nz;

		this.Normals = p_normal;
	}

	public void FixData(PrimPoint[] points) {
		if (this is IPrimFace) {
			this.FixFacesPart1();
			this.FixFacesPart2();
			this.CalcNormal(points);
		}
	}
}
