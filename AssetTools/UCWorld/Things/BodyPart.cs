using AssetTools.UCWorld.Poly;
using Godot;
using System.Collections.Generic;

namespace AssetTools.UCWorld.Things;

public class BodyPart
{
	public List<IPoly> Faces { get; set; }

	public Quaternion Rotation { get; set; }

	public Vector3 Offset { get; set; }
}
