namespace AssetTools.UCWorld.Poly;

/// <summary>
/// Declares a generic polygon to be drawn. This may be a triangle, a quad, etc.
/// The output must always be ready to form Triangle-based surfaces
/// </summary>
public interface IPoly
{
	/// <summary>
	/// List of vertices that forms this poligon
	/// </summary>
	/// <returns></returns>
	MapVertex[] GetVertices();

	/// <summary>
	/// Texture ID to be used
	/// </summary>
	/// <returns></returns>
	int GetTexturePage();

	/// <summary>
	/// Indices to be drawn. The values must refer to indices of GetVertices
	/// </summary>
	/// <returns></returns>
	int[] GetIndices();
}
