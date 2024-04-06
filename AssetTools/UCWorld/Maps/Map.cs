using AssetTools.Structures;
using AssetTools.UCFileStructures.Maps;
using System.Collections.Generic;

namespace AssetTools.UCWorld.Maps;

/// <summary>
/// A Urban Chaos Map in its "Map" size representation.
/// Contains every information about it in a structured way that is easier for renderers to use
/// </summary>
public class UCMap
{
	public FileContainer<Iam> Iam { get; set; }

	public List<FloorFace> FloorFaces { get; set; }

	public List<Facet> Facets { get; set; }
}
