using AssetTools.Structures;
using AssetTools.UCFileStructures.MultiPrim;
using System.Collections.Generic;

namespace AssetTools.UCWorld.Things;

public class Thing
{
	public FileContainer<AllFile> All { get; set; }

	public List<BodyPart> BodyParts { get; set; }
}
