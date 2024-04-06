using AssetTools.Structures;
using AssetTools.UCFileStructures.Tma;

namespace AssetTools.UCWorld.Textures;

public class TextureStyle
{
	public AssetLoadStatus TmaStatus { get; set; } = AssetLoadStatus.NotLoaded;
	public string TmaFilePath { get; set; } = "";
	public StyleTma TmaFile { get; set; } = null;

	public DxTextureXY[][] DxTextureXYs { get; set; }

	public TextureStyle(StyleTma tma) {
		this.TmaFile = tma;
		this.DxTextureXYs = new DxTextureXY[200][];
		for (int style = 0; style < 200; style++) {
			this.DxTextureXYs[style] = new DxTextureXY[5];
			for (int piece = 0; piece < 5; piece++) {
				this.DxTextureXYs[style][piece] = DxTextureXY.FromTextureXY(tma.TextureXYSection.TextureXYs[style][piece]);
			}
		}
	}
}
