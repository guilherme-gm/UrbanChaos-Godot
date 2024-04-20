using AssetTools.Structures;
using AssetTools.UCFileStructures.MultiPrim;
using AssetTools.UCFileStructures.Prim;
using AssetTools.UCWorld.Poly;
using Godot;
using System;
using System.IO;

namespace AssetTools.UCWorld.Things;

public class ThingLoader
{
	private Thing Thing;

	private AllFile All => this.Thing.All.Data;

	public class D3dVertex
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float Nx { get; set; }
		public float Ny { get; set; }
		public float Nz { get; set; }
		public float TU { get; set; }
		public float TV { get; set; }
	}

	public class TomsPrimObject
	{
		public ushort WFlags { get; set; } = 0;                      // D3DOBJECT_FLAG_*** flags.
		public ushort WNumMaterials { get; set; }                // Number of materials.
		public ushort WTotalSizeOfObj { get; set; }          // Number of vertices used by object.
		public byte BLRUQueueNumber { get; set; }          // Position in the LRU queue.
		public byte BPadding { get; set; }
		public /* PrimObjectMaterial* */ object[] PMaterials { get; set; } // Pointer to the materials. Can MemFree this.
		public /* void* */ object[] PD3DVertices { get; set; }             // Pointer to the D3DVERTEX list. DONT MEMFREE THIS
		public /* UWORD* */ ushort[] PwListIndices { get; set; }               // Pointer to the indices in list form. Can MemFree this.
		public /* UWORD* */ ushort[] PwStripIndices { get; set; }          // Pointer to the indices in interrupted strip form. DONT MEMFREE THIS
		public float FBoundingSphereRadius { get; set; } = 0f;        // Guess!

		public D3dVertex[] TPOVerts { get; set; }
		public ushort[] TPOStripIndices { get; set; }
		public ushort[] TPOListIndices { get; set; }
		public int[] TPOVertexRemap { get; set; }
		public int[] TPOVertexLinks { get; set; }

		public int[] TPOPrimObjIndexOffset { get; set; } = new int[17];

		public int TPONumListIndices { get; set; } = 0;
		public int TPONumStripIndices { get; set; } = 0;
		public int TPONumVertices { get; set; } = 0;
		public int TPONumPrims { get; set; } = 0;

		public MultiObjectPrim[] TPOPrimObjects { get; set; } = new MultiObjectPrim[200]; // 16
		public byte[] TPOUbPrimObjMMIndex { get; set; } = new byte[200]; // 16

		// FIGURE_TPO_init_3d_object
		public TomsPrimObject() {
			int MaxVerts = 1024;
			int MaxIndices = MaxVerts * 4;
			this.TPOVerts = new D3dVertex[MaxVerts];
			this.TPOStripIndices = new ushort[MaxIndices];
			this.TPOListIndices = new ushort[MaxIndices];
			this.TPOVertexRemap = new int[MaxVerts];
			this.TPOVertexLinks = new int[MaxVerts];
		}

		// FIGURE_TPO_add_prim_to_current_object
		// Add a prim to this 3D object.
		// prim = the prim number to add.
		// iSubObjectNumber = sub-object number, used for multimatrix stuff.
		//		Start at 0 and go up by one for each object.
		public void AddPrimtToCurrentObject(MultiObjectPrim prim, byte ubSubOjectNumber) {
			GD.Print(this.TPONumPrims);
			this.TPOPrimObjects[this.TPONumPrims] = prim;
			this.TPOUbPrimObjMMIndex[this.TPONumPrims] = ubSubOjectNumber;
			// this.TPOPrimObjIndexOffset[this.TPONumPrims + 1] = this.TPOPrimObjIndexOffset[this.TPONumPrims] + (prim.EndPoint - prim.StartPoint);

			this.TPONumPrims++;
		}

		public void Finish3dObject(int trashIndex = 0) {
			_ = trashIndex;

			// @TODO: Implement this for real textures...

			// Draw order:
			// Triangle - 0, 1 , 2
			// Quad - 0, 1, 2, 2 , 1 , 3

			for (int outerPrimNumber = 0; outerPrimNumber < this.TPONumPrims; outerPrimNumber++) {
				var outerObj = this.TPOPrimObjects[outerPrimNumber];
				var outerTris = false;

				do {
					int outerFaceNum;
					int outerFaceEnd;
					if (outerTris) {
						outerFaceNum = outerObj.StartFace3;
						outerFaceEnd = outerObj.EndFace3;
					} else {
						outerFaceNum = outerObj.StartFace4;
						outerFaceEnd = outerObj.EndFace4;
					}

					for (; outerFaceNum < outerFaceEnd; outerFaceNum++) {
						// @TODO:
						// UWORD wTexturePage = FIGURE_find_face_D3D_texture_page(iOuterFaceNum, bOuterTris);
						// int wTexturePage = 1;

						// // Find the _rendered_ page, to allow texture paging to do its stuff.
						// UWORD wRealPage = wTexturePage & TEXTURE_PAGE_MASK;
						// int wRealPage = 1;

					}

					outerTris = !outerTris;
				} while (outerTris);
			}
		}
	};

	public class DhprRData1
	{
		public int PartNumber { get; set; }
		public int CurrentChildNumber { get; set; }
		// CMatrix33 *parent_base_mat;
		// Matrix31 *parent_base_pos;
		// Matrix33 *parent_current_mat;
		// Matrix31 *parent_current_pos;
		// Matrix31 pos;
	}

	private readonly int[][] BodyPartChildren = [
		[ 1, 4, 12, -1, 0 ],
		[ 2, -1, 0, 0, 0 ],
		[ 3, -1, 0, 0, 0 ],
		[ -1, 0, 0, 0, 0 ],
		[ 5, 8, 11, -1, 0 ],
		[ 6, -1, 0, 0, 0 ],
		[ 7, -1, 0, 0, 0 ],
		[ -1, 0, 0, 0, 0 ],
		[ 9, -1, 0, 0, 0 ],
		[ 10, -1, 0, 0, 0 ],
		[ -1, 0, 0, 0, 0 ],
		[ -1, 0, 0, 0, 0 ],
		[ 13, -1, 0, 0, 0 ],
		[ 14, -1, 0, 0, 0 ],
		[ -1, 0, 0, 0, 0 ]
	];

	private MapVertex ConvertPoint(PrimPoint point, GameKeyFrameElement ele, GameKeyFrameElement parent) {
		return new MapVertex() {
			Position = new Vector3(
				(point.X + /* ele.OffsetX */ 0 /* (parent?.OffsetX ?? 0) */) * 256,
				(point.Y + /* ele.OffsetY */ 0 /* (parent?.OffsetY ?? 0) */) * 256,
				(point.Z + /* ele.OffsetZ */ 0 /* (parent?.OffsetZ ?? 0) */) * 256
			)
		};
	}

	private int GetTexturePage(PrimFace face) {
		var texturePage = face.TexturePage;
		int drawFlags;
		int faceFlags;

		if (face is PrimFace3 face3) {
			drawFlags = face3.DrawFlags;
			faceFlags = face3.FaceFlags;
			// texturePage = (ushort)(face3.CompressedUV[0][0] & 0xc0);
			// texturePage <<= 2;
			// texturePage = (ushort)(texturePage | face3.TexturePage);
		} else if (face is PrimFace4 face4) {
			drawFlags = face4.DrawFlags;
			faceFlags = face4.FaceFlags;
			// texturePage = (ushort)(face4.CompressedUV[0][0] & 0xc0);
			// texturePage <<= 2;
			// texturePage = (ushort)(texturePage | face4.TexturePage);
		} else {
			GD.PushWarning("GetTexturePage: Not PrimFace3/PrimFace4");
			return 0;
		}

		// GD.Print($"{face.TexturePage} , {texturePage}");

		if ((drawFlags & 2 /* POLY_FLAG_TEXTURED */) == 0) {
			// Untextured
			texturePage = 4096 /*TEXTURE_PAGE_FLAG_NOT_TEXTURED*/ | 1432 /* POLY_PAGE_COLOUR */;
			GD.Print("Untextured found");
		} else {
			// Textured

			if ((faceFlags & 32768 /* FACE_FLAG_THUG_JACKET */) != 0) {
				GD.Print("Thug Jacket");
				switch (texturePage) {
					case 85:
					case 642:
					case 672:
						texturePage = 0 | 32768 /* TEXTURE_PAGE_FLAG_JACKET */;
						break;

					case 64 + 22:
					case (10 * 64) + 3:
					case (10 * 64) + 33:
						texturePage = 1 | 32768 /* TEXTURE_PAGE_FLAG_JACKET */;
						// page=jacket_lookup[1][GET_SKILL(p_thing)>>2];
						break;
					case 64 + 24:
					case (10 * 64) + 4:
					case (10 * 64) + 36:
						texturePage = 2 | 32768 /* TEXTURE_PAGE_FLAG_JACKET */;
						// page=jacket_lookup[2][GET_SKILL(p_thing)>>2];
						break;
					case 64 + 25:
					case (10 * 64) + 5:
					case (10 * 64) + 37:
						texturePage = 3 | 32768 /* TEXTURE_PAGE_FLAG_JACKET */;
						// page=jacket_lookup[3][GET_SKILL(p_thing)>>2];
						break;

					default:
						GD.PushError("Unhandled jacket texture page: " + texturePage);
						return 0;
				}
			} else if ((texturePage > 640)) {
				GD.Print($"Test: {texturePage - (10 * 64)}");
			} else {
				texturePage += 512 /* FACE_PAGE_OFFSET */;
				GD.Print("General purpose" + texturePage);
			}
		}

		if ((faceFlags & 256) != 0) {
			GD.Print("Texture_page_flag_tint");
			texturePage |= 8192 /* Texture_page_flag_tint */;
		}

		return texturePage;
	}


	private int GetTexturePage2(PrimFace face) {
		GD.Print(face.TexturePage);
		return face.TexturePage;
		// var wTexturePage = this.GetTexturePage(face);
		// var wRealPage = wTexturePage & 0x0fff /* TEXTURE_PAGE_MASK */ ;

		// GD.Print($"Real texture: {wRealPage}");
		// if ((wTexturePage & (1 << 15) /* TEXTURE_PAGE_FLAG_JACKET */) != 0) {
		// 	GD.Print("Jacket");
		// } else if ((wTexturePage & (1 << 14) /* TEXTURE_PAGE_FLAG_OFFSET */) != 0) {
		// 	GD.Print("TEXTURE_PAGE_FLAG_OFFSET");
		// 	wRealPage += 8 * 64 /* FACE_PAGE_OFFSET */;
		// }

		// return wRealPage;
	}

	private GameKeyFrameElement DrawElement(BodyPart part, GameKeyFrameElement ele, MultiObjectPrim prim, GameKeyFrameElement parent) {
		foreach (var face3 in prim.PrimFace3s) {
			var vertex1 = this.ConvertPoint(prim.PrimPoints[face3.Points[0]], ele, parent);
			var vertex2 = this.ConvertPoint(prim.PrimPoints[face3.Points[1]], ele, parent);
			var vertex3 = this.ConvertPoint(prim.PrimPoints[face3.Points[2]], ele, parent);
			part.Faces.Add(new TrianglePoly(vertex1, vertex2, vertex3, this.GetTexturePage2(face3)));
		}

		foreach (var face4 in prim.PrimFace4s) {
			var vertex1 = this.ConvertPoint(prim.PrimPoints[face4.Points[0]], ele, parent);
			var vertex2 = this.ConvertPoint(prim.PrimPoints[face4.Points[1]], ele, parent);
			var vertex3 = this.ConvertPoint(prim.PrimPoints[face4.Points[2]], ele, parent);
			var vertex4 = this.ConvertPoint(prim.PrimPoints[face4.Points[3]], ele, parent);
			part.Faces.Add(new PersonPoly(vertex1, vertex2, vertex3, vertex4, this.GetTexturePage2(face4)));
		}

		return ele;
	}

	private Quaternion QuatSlerp(Quaternion from, Quaternion to, float t) {
		float[] to1 = [0, 0, 0, 0];
		double omega, cosom, sinom, scale0, scale1;

		cosom = (from.X * to.X) + (from.Y * to.Y) + (from.Z * to.Z) + (from.W * to.W);
		if (cosom < 0f) {
			cosom = -cosom;
			to1[0] = -to.X;
			to1[1] = -to.Y;
			to1[2] = -to.Z;
			to1[3] = -to.W;
		} else {
			to1[0] = to.X;
			to1[1] = to.Y;
			to1[2] = to.Z;
			to1[3] = to.W;
		}

		if ((1f - cosom) > 0.05f) {
			omega = (float)Math.Acos(cosom);
			sinom = (float)Math.Sin(omega);
			scale0 = (float)Math.Sin((1f - t) * omega) / sinom;
			scale1 = (float)Math.Sin(t * omega) / sinom;
		} else {
			scale0 = 1f - t;
			scale1 = t;
		}

		return new Quaternion(
			(float)((scale0 * from.X) + (scale1 * to1[0])),
			(float)((scale0 * from.Y) + (scale1 * to1[1])),
			(float)((scale0 * from.Z) + (scale1 * to1[2])),
			(float)((scale0 * from.W) + (scale1 * to1[3]))
		);
	}

	private bool CheckIsoNormal(FloatMatrix m) {
		if ((m.M[0][0] == 0) && (m.M[0][1] == 0) && (m.M[0][2] == 0))
			return true; // void matrix

		// check handedness
		float x = (m.M[0][1] * m.M[1][2]) - (m.M[0][2] * m.M[1][1]);
		float y = (m.M[0][2] * m.M[1][0]) - (m.M[0][0] * m.M[1][2]);
		float z = (m.M[0][0] * m.M[1][1]) - (m.M[0][1] * m.M[1][0]);

		if ((Math.Abs(x - m.M[2][0]) > 0.03) ||
			(Math.Abs(y - m.M[2][1]) > 0.03) ||
			(Math.Abs(z - m.M[2][2]) > 0.03)) {
			return false;
		}

		return true;
	}

	private void ConvertPerson() {
		TomsPrimObject primObject = new TomsPrimObject();
		byte tpoPartNumber = 0;
		int recurseLevel = 0;

		// if (numMAterials == 0) -- mesh not created yet. We never have one anyway.

		DhprRData1[] dhprRData1s = [
			new DhprRData1(),
			new DhprRData1(),
			new DhprRData1(),
			new DhprRData1(),
			new DhprRData1()
		];
		dhprRData1s[0] = new DhprRData1() {
			PartNumber = 0,
			CurrentChildNumber = 0,
		};

		this.Thing.BodyParts = [];

		GameKeyFrameElement[] eles = new GameKeyFrameElement[5];

		var eleIdx = this.All.GameChunk.AnimKeyFrames[1].FirstElementIdx;
		var test = 0;
		while (recurseLevel >= 0) {
			var dhpr1 = dhprRData1s[recurseLevel];
			int partNumber = dhpr1.PartNumber;
			if (dhpr1.CurrentChildNumber == 0) {
				// @TODO: 0 = Darci ... need to handle others
				int bodyPart = this.All.GameChunk.PeopleTypes[0].BodyParts[partNumber];
				// Looks like for Darci it is 0, but the meaning of this is still confusing to me...
				var pObj = this.All.MultiObjects[0].Objects[bodyPart];
				primObject.AddPrimtToCurrentObject(pObj, tpoPartNumber);
				tpoPartNumber++;

				var bodyPartObj = new BodyPart {
					Faces = []
				};

				this.Thing.BodyParts.Add(bodyPartObj);

				if (recurseLevel > 0) {
					eles[recurseLevel] = this.DrawElement(bodyPartObj, this.All.GameChunk.TheElements[eleIdx + partNumber], pObj, null /*eles[recurseLevel - 1]*/);
				} else {
					eles[recurseLevel] = this.DrawElement(bodyPartObj, this.All.GameChunk.TheElements[eleIdx + partNumber], pObj, null);
				}

				bodyPartObj.Offset = new Vector3(
					this.All.GameChunk.TheElements[eleIdx + partNumber].OffsetX,
					this.All.GameChunk.TheElements[eleIdx + partNumber].OffsetY,
					this.All.GameChunk.TheElements[eleIdx + partNumber].OffsetZ
				);

				var fmat1 = new FloatMatrix(this.All.GameChunk.TheElements[eleIdx + partNumber].CMatrix);
				var quat1 = fmat1.ToQuaternion();
				var fmat2 = new FloatMatrix(this.All.GameChunk.TheElements[eleIdx + partNumber].CMatrix);
				var quat2 = fmat2.ToQuaternion();

				var a = this.CheckIsoNormal(fmat1);
				var b = this.CheckIsoNormal(fmat2);
				if (a != b) {
					GD.Print("a and b is diff");
				}

				bodyPartObj.Rotation = this.QuatSlerp(quat1, quat2, 64 / 256f);
			}

			GD.Print($"{partNumber} - {dhpr1.CurrentChildNumber} - {this.BodyPartChildren[partNumber][dhpr1.CurrentChildNumber]}");
			if (this.BodyPartChildren[partNumber][dhpr1.CurrentChildNumber] != -1) {
				var dhpr1Inc = dhprRData1s[recurseLevel + 1];
				dhpr1Inc.CurrentChildNumber = 0;
				dhpr1Inc.PartNumber = this.BodyPartChildren[partNumber][dhpr1.CurrentChildNumber];
				dhpr1.CurrentChildNumber++;
				recurseLevel++;
			} else {
				recurseLevel--;
			}

			test++;
			// if (test == 5)
			// 	break;
		}

		// for (int i = 0; i < primObject.TPONumPrims; i++) {
		// 	var prim = primObject.TPOPrimObjects[i];
		// 	var partNumber = primObject.TPOUbPrimObjMMIndex[i];
		// 	this.DrawElement(this.All.GameChunk.TheElements[eleIdx + partNumber], prim);

		// }

		// // Compile the whole object now.
		// // Use thrash slot 1 because the gun may bump it out later if it's
		// // in slot 0!
		// primObject.Finish3dObject(1);

		// Triangle - 0, 1 , 2
		// Quad - 0, 1, 2, 2 , 1 , 3
		var moveCnt = 0;
		// foreach (var multiObj in this.All.MultiObjects) {
		// 	foreach (var prim in multiObj.Objects) {
		// 		// foreach (var prim in this.All.MultiObjects[1].Objects) {
		// 			// var prim = this.All.MultiObjects[1].Objects[14]; // .TPOPrimObjects[i];
		// 			foreach (var face3 in prim.PrimFace3s) {
		// 				var vertex1 = this.ConvertPoint(prim.PrimPoints[face3.Points[0]], Vector3.Left * 50 * moveCnt);
		// 				var vertex2 = this.ConvertPoint(prim.PrimPoints[face3.Points[1]], Vector3.Left * 50 * moveCnt);
		// 				var vertex3 = this.ConvertPoint(prim.PrimPoints[face3.Points[2]], Vector3.Left * 50 * moveCnt);
		// 				this.Thing.Faces.Add(new TrianglePoly(vertex1, vertex2, vertex3, this.GetTexturePage2(face3)));
		// 			}

		// 			foreach (var face4 in prim.PrimFace4s) {
		// 				var vertex1 = this.ConvertPoint(prim.PrimPoints[face4.Points[0]], Vector3.Left * 50 * moveCnt);
		// 				var vertex2 = this.ConvertPoint(prim.PrimPoints[face4.Points[1]], Vector3.Left * 50 * moveCnt);
		// 				var vertex3 = this.ConvertPoint(prim.PrimPoints[face4.Points[2]], Vector3.Left * 50 * moveCnt);
		// 				var vertex4 = this.ConvertPoint(prim.PrimPoints[face4.Points[3]], Vector3.Left * 50 * moveCnt);
		// 				this.Thing.Faces.Add(new PersonPoly(vertex1, vertex2, vertex3, vertex4, this.GetTexturePage2(face4)));
		// 			}
		// 		// }
		// 		moveCnt++;
		// 	}

		// 	break;
		// }

		// for (int i = 0; i < primObject.TPONumPrims; i++) {
		// 	var prim = primObject.TPOPrimObjects[i];
		// 	foreach (var face3 in prim.PrimFace3s) {
		// 		var vertex1 = this.ConvertPoint(prim.PrimPoints[face3.Points[0]]);
		// 		var vertex2 = this.ConvertPoint(prim.PrimPoints[face3.Points[1]]);
		// 		var vertex3 = this.ConvertPoint(prim.PrimPoints[face3.Points[2]]);
		// 		this.Thing.Faces.Add(new TrianglePoly(vertex1, vertex2, vertex3));
		// 	}

		// 	foreach (var face4 in prim.PrimFace4s) {
		// 		var vertex1 = this.ConvertPoint(prim.PrimPoints[face4.Points[0]]);
		// 		var vertex2 = this.ConvertPoint(prim.PrimPoints[face4.Points[1]]);
		// 		var vertex3 = this.ConvertPoint(prim.PrimPoints[face4.Points[2]]);
		// 		var vertex4 = this.ConvertPoint(prim.PrimPoints[face4.Points[3]]);
		// 		this.Thing.Faces.Add(new QuadPoly(vertex1, vertex2, vertex3, vertex4));
		// 	}
		// }

		// Huh? Why discard the first one? the original code really does that...
		// dhprRData1s[0] = new DhprRData1() {
		// 	PartNumber = 0,
		// 	CurrentChildNumber = 0,
		// };

		// // --- All the time
		// tpoPartNumber = -1;
		// var wholePersonVisible = true;
		// var bitsOfPersonVisible = false;

		// recurseLevel = 0;
		// while (recurseLevel >= 0) {
		// 	var dhpr1 = dhprRData1s[recurseLevel];
		// 	int partNumber = dhpr1.PartNumber;

		// 	if (dhpr1.CurrentChildNumber == 0) {
		// 		int bodyPart = this.All.GameChunk.PeopleTypes[0].BodyParts[partNumber];
		// 		// int rotMat = 

		// 		tpoPartNumber++;
		// 		int personType = 0;
		// 		int personID = 0;
		// 		if (personType != 1 && personID != 0) {
		// 			var id = personID >> 5;
		// 			int hand = id == 2 ? 10 /* SUB_OBJECT_RIGHT_HAND */ : 7 /* SUB_OBJECT_LEFT_HAND */;

		// 			if (partNumber == hand) {
		// 				// @TODO: Gunflash


		// 			}
		// 		}
		// 	}
		// }
	}

	private void ConvertPlayer() {
		if (this.Thing.All?.Status != AssetLoadStatus.Loaded) {
			return;
		}

		// @TODO: Tweened is not properly set...

		// int anim = 3; // ANIM_STAND_READY

		// @TODO: Color

		// @TODO: Pyro

		if (this.All.GameChunk.ElementCount == 15) {
			this.ConvertPerson();
		} else {
			GD.PushWarning("@TODO: Convert game chunk of ElementCount != 15");
		}
	}

	public Thing LoadFomFile(string path) {
		this.Thing = new Thing();

		try {
			if (!File.Exists(path)) {
				this.Thing.All = new FileContainer<AllFile>(null, path, AssetLoadStatus.NotFound);
				return this.Thing;
			}

			using var fs = new FileStream(path, FileMode.Open);
			using var br = new BinaryReader(fs);

			var allFile = AllFile.Deserialize(br);
			this.Thing.All = new FileContainer<AllFile>(allFile, path, AssetLoadStatus.Loaded);

			// @TODO: setup_global_anim_array()

			this.ConvertPlayer();

			return this.Thing;
		}
		catch (Exception exception) {
			GD.PushError($"Failed to load anim file. {exception.Message}");
			GD.PushError(exception);
			if (this.Thing.All == null) {
				this.Thing.All = new FileContainer<AllFile>(null, path, AssetLoadStatus.Error);
			} else {
				this.Thing.All.Status = AssetLoadStatus.Error;
			}
			return this.Thing;
		}
	}
}
