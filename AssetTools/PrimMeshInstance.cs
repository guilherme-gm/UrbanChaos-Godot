using AssetTools.UCFileStructures.Prim;
using Godot;
using System.Collections.Generic;
using System.IO;

namespace AssetTools;

[Tool]
public partial class PrimMeshInstance : MeshInstance3D
{
	private string PrimFilePath;

	[Export(PropertyHint.GlobalFile)]
	public string PrimPath {
		get => this.PrimFilePath;
		set {
			this.PrimFilePath = value;
			this.LoadPrim(this.PrimFilePath);
		}
	}

	private void DrawPart(ArrayMesh arrMesh, PrimPoint[] points, PrimFace3[] faces3, PrimFace4[] faces4) {
		int idx = 0;

		foreach (var face in faces3) {
			var triangleSurfaces = new Godot.Collections.Array();
			_ = triangleSurfaces.Resize((int)Mesh.ArrayType.Max);

			var triangleVerts = new List<Vector3>() { };
			var triangleUVs = new List<Vector2>() { };
			var triangleNormals = new List<Vector3>() { };
			var triangleIndices = new List<int>() { };

			int triangleVertsCount = 0;

			int i = 0;
			foreach (var pointId in face.Points) {
				var point = points[pointId];

				triangleVerts.Add(new Vector3(point.X, point.Y, point.Z));
				triangleIndices.Add(triangleVertsCount);
				triangleVertsCount++;

				triangleNormals.Add(face.Normals);
				triangleUVs.Add(new Vector2(face.UV[i][0], face.UV[i][1]));

				i++;
			}

			triangleSurfaces[(int)Mesh.ArrayType.Vertex] = triangleVerts.ToArray();
			triangleSurfaces[(int)Mesh.ArrayType.TexUV] = triangleUVs.ToArray();
			triangleSurfaces[(int)Mesh.ArrayType.Normal] = triangleNormals.ToArray();
			triangleSurfaces[(int)Mesh.ArrayType.Index] = triangleIndices.ToArray();

			arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, triangleSurfaces);
			// var textureHolder = this.Textures.First((v) => v.IdVal == face.RealPage);
			// arrMesh.SurfaceSetMaterial(idx, textureHolder.TextureVal);
			idx++;
		}

		foreach (var face in faces4) {
			int rectVertsCount = 0;

			var rectSurfaces = new Godot.Collections.Array();
			_ = rectSurfaces.Resize((int)Mesh.ArrayType.Max);

			var rectVerts = new List<Vector3>() { };
			var rectUVs = new List<Vector2>() { };
			var rectNormals = new List<Vector3>() { };
			var rectIndices = new List<int>() { };

			int i = 0;
			foreach (var pointId in face.Points) {
				var point = points[pointId];

				rectVerts.Add(new Vector3(point.X, point.Y, point.Z));
				rectIndices.Add(rectVertsCount);
				rectVertsCount++;

				rectNormals.Add(face.Normals);
				rectUVs.Add(new Vector2(face.UV[i][0], face.UV[i][1]));

				i++;
			}

			rectSurfaces[(int)Mesh.ArrayType.Vertex] = rectVerts.ToArray();
			rectSurfaces[(int)Mesh.ArrayType.TexUV] = rectUVs.ToArray();
			rectSurfaces[(int)Mesh.ArrayType.Normal] = rectNormals.ToArray();
			rectSurfaces[(int)Mesh.ArrayType.Index] = rectIndices.ToArray();

			arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.TriangleStrip, rectSurfaces);
			// var textureHolder = this.Textures.First((v) => v.IdVal == face.RealPage);
			// arrMesh.SurfaceSetMaterial(idx, textureHolder.TextureVal);
			idx++;
		}

	}

	private void DrawPrim(IPrim prim) {
		var st = new SurfaceTool();
		st.Begin(Mesh.PrimitiveType.Triangles);

		var arrMesh = new ArrayMesh();

		this.DrawPart(arrMesh, prim.PrimPoints, prim.PrimFace3s, prim.PrimFace4s);

		this.Mesh = arrMesh;
	}

	public void LoadPrim(string path) {
		if (path == "") {
			return;
		}

		this.PrimFilePath = path;

		var fileName = Path.GetFileName(path).ToLower();
		IPrim prim;

		using var fs = new FileStream(path, FileMode.Open);
		using var br = new BinaryReader(fs);
		if (fileName.StartsWith("nprim")) {
			prim = NPrimFile.Deserialize(br);
		} else if (fileName.StartsWith("prim")) {
			prim = PrimFile.Deserialize(br);
		} else {
			throw new System.Exception($"Could not identify file type for {fileName}");
		}

		this.DrawPrim(prim);
	}

	public void LoadPrim(IPrim prim) {
		this.PrimFilePath = null;
		this.DrawPrim(prim);
	}
}
