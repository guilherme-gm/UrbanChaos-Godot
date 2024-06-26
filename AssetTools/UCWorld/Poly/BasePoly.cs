namespace AssetTools.UCWorld.Poly;

/// <summary>
/// A 4 point quad used as a Strip. This is used for map floors.
/// In original code this is generated by calling draw_i_prim or DrawIndPrimMM with certain parameters
/// 
/// Note that while this looks like QuadPoly, the Indexes are different.
/// </summary>
public abstract class BasePoly
{
	protected MapVertex[] MapVertices;

	protected int TexturePage;

	protected BasePoly() { }

	public int GetTexturePage() => this.TexturePage;

	public MapVertex[] GetVertices() => this.MapVertices;
}
