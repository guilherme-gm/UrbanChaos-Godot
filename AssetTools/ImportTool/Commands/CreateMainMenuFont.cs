using AssetTools.AssetManagers;
using Godot;
using System;
using System.IO;

namespace AssetTools.ImportTool.Commands;

public class CreateMainMenuFont : ICommand
{
	private string FromTga { get; set; }

	private string ToRes { get; set; }

	public CreateMainMenuFont(string fromTga, string toRes) {
		this.FromTga = fromTga;

		this.ToRes = toRes;
	}

	public void Execute() {
		var fromPath = Path.Join(AssetPathManager.Instance.UCFolderPath, this.FromTga);
		var toPath = Path.Join(AssetPathManager.Instance.WorkFolderPath, this.ToRes);

		_ = Directory.CreateDirectory(Path.GetDirectoryName(toPath));

		var img = new Image();
		_ = img.Load(fromPath);

		// Vector2I (fontSize, outline)
		var size = new Vector2I(30, 0);
		var fnt = new FontFile();
		fnt.SetTextureImage(0, size, 0, img);
		char[][] glyphs = [
			['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'],
			['I', 'J', 'K', 'L', 'M', 'N', 'O', 'P'],
			['Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X'],
			['Y', 'Z', '0', '1', '2', '3', '4', '5'],
			['6', '7', '8', '9', '.', ',', '!', '"'],
			[':', ';', '\'', '#', '$', '*', '_', '('],
			[')', '[', ']', '\\', '/', '?', 'È', 'À'],
			['Ü', 'Ö', 'Ä', 'Ù', 'Ú', 'Ó', 'Á', 'É'],
		];

		for (int col = 0; col < glyphs.Length; col++) {
			for (int line = 0; line < glyphs[col].Length; line++) {
				var glyph = glyphs[col][line];
				fnt.SetGlyphAdvance(0, size.X, glyph, new Vector2(30, 30)); // This means how much space the character uses in X. I am not sure about Y
				fnt.SetGlyphOffset(0, size, glyph, new Vector2(0, -15)); // I am not sure what this does, but it seems related to "centering" the character.
				fnt.SetGlyphSize(0, size, glyph, new Vector2(30, 30)); // This means the size of the drawn image
				fnt.SetGlyphTextureIdx(0, size, glyph, 0); // I think this binds it to the SetTextureImage
				fnt.SetGlyphUVRect(0, size, glyph, new Rect2(3 + (line * 31), 1 + (col * 31), 30, 30)); // This maps the region from the image into the character

				// Make lower case letters use the uppercase ones (we don't differentiate them)
				if (Char.ToLower(glyph) != glyph) {
					var lowerGlyph = Char.ToLower(glyph);
					fnt.SetGlyphAdvance(0, size.X, lowerGlyph, new Vector2(30, 30));
					fnt.SetGlyphOffset(0, size, lowerGlyph, new Vector2(0, -15));
					fnt.SetGlyphSize(0, size, lowerGlyph, new Vector2(30, 30));
					fnt.SetGlyphTextureIdx(0, size, lowerGlyph, 0);
					fnt.SetGlyphUVRect(0, size, lowerGlyph, new Rect2(3 + (line * 31), 1 + (col * 31), 30, 30));
				}
			}
		}

		// Sets space character
		fnt.SetGlyphAdvance(0, size.X, ' ', new Vector2(30, 0));
		fnt.SetGlyphSize(0, size, ' ', new Vector2(30, 0));

		var result = ResourceSaver.Singleton.Save(fnt, toPath);
		if (result != Error.Ok) {
			throw new Exception($"Failed to create font. Error: {result}");
		}
	}

	public string GetLog() {
		return $"Creating main menu font from \"{this.FromTga}\" to \"{this.ToRes}\"";
	}
}
