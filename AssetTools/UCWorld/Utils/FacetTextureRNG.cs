namespace AssetTools.UCWorld.Utils;

public class FacetTextureRNG
{
	private uint Seed;

	public FacetTextureRNG(uint highResX, uint highResY, uint highResZ) {
		this.Seed = (highResX * highResZ) + highResY;
	}

	public uint Next() {
		this.Seed = (this.Seed * 69069) + 1;

		return this.Seed >> 7;
	}
}
