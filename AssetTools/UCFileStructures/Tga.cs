using System;
using System.IO;

namespace AssetTools.UCFileStructures;

public partial class Tga
{
	private static readonly byte[] C8to4 = new byte[256];
	private static readonly byte[] C8to5 = new byte[256];
	private static readonly byte[] C8to6 = new byte[256];
	private static readonly byte[] C4to8 = new byte[16];
	private static readonly byte[] C5to8 = new byte[32];
	private static readonly byte[] C6to8 = new byte[64];

	static Tga() {
		int ii;

#pragma warning disable IDE0047 // Remove unnecessary parentheses
		for (ii = 0; ii < 256; ii++) {
			C8to4[ii] = (byte)((((ii * 30) / 256) + 1) / 2);
			C8to5[ii] = (byte)((((ii * 62) / 256) + 1) / 2);
			C8to6[ii] = (byte)((((ii * 126) / 256) + 1) / 2);
		}
#pragma warning restore IDE0047 // Remove unnecessary parentheses		

		for (ii = 0; ii < 16; ii++) C4to8[ii] = (byte)((ii << 4) | ii);
		for (ii = 0; ii < 32; ii++) C5to8[ii] = (byte)((ii << 3) | (ii >> 2));
		for (ii = 0; ii < 64; ii++) C6to8[ii] = (byte)((ii << 2) | (ii >> 4));
	}

	public struct TgaPixel
	{
		public byte blue;
		public byte green;
		public byte red;
		public byte alpha;
	}

	public ushort ContainsAlpha { get; set; }

	public ushort Width { get; set; }

	public ushort Height { get; set; }

	public TgaPixel[] PixelData { get; set; }

	public int OverreadCount { get; set; }

	public static byte[] ReadSquished(BinaryReader br, out int overreadCount) {
		ushort header = br.ReadUInt16();
		overreadCount = 0;
		if (header != 0xFFFF) {
			_ = br.BaseStream.Seek(0, SeekOrigin.Begin);
			using var memStream = new MemoryStream();

			br.BaseStream.CopyTo(memStream);
			return memStream.ToArray();
		}

		ushort headerA = br.ReadUInt16();
		ushort sizeA = br.ReadUInt16();
		ushort sizeB = br.ReadUInt16();

		int nwords = sizeA * sizeB;
		int bufferSize = 2 * (nwords + 3);

		byte[] output = new byte[bufferSize];
		var bw = new BinaryWriter(new MemoryStream(output));

		// Copy header
		bw.Write(headerA);
		bw.Write(sizeA);
		bw.Write(sizeB);

		// read mapping
		ushort total = br.ReadUInt16();
		ushort[] mapping = new ushort[65536];

		for (int i = 0; i < total; i++) {
			mapping[i] = br.ReadUInt16();
		}

		int bits = 1;

#pragma warning disable IDE0047 // Remove unnecessary parentheses
		while (total > (1 << bits))
			bits++;
#pragma warning restore IDE0047 // Remove unnecessary parentheses

		// read bit-encoded data
		int cbits = 16;
		ushort cword = br.ReadUInt16();

		for (uint i = 0; i < nwords; i++) {
			ushort encoded;

			if (cbits > bits) {
				// read (bits) bits
				encoded = (ushort)(cword >> (16 - bits));
				cword <<= bits;
				cbits -= bits;
			} else {
				// read (16 - cbits) bits
				int xbits = bits - cbits;
				encoded = (ushort)(cword >> (16 - bits));

				if (br.BaseStream.Position < br.BaseStream.Length) {
					cword = br.ReadUInt16();
				} else {
					cword = 0;
					overreadCount++;
				}

				cbits = 16;
				encoded |= (ushort)(cword >> (16 - xbits));
				cword = (ushort)(cword << xbits);
				cbits -= xbits;
			}

			if (encoded >= total) {
				throw new Exception($"Reading failed. (Expected encoded ({encoded}) < total ({total})");
			}

			bw.Write(mapping[encoded]);
		}

		return output;
	}

	public static Tga Deserialize(BinaryReader br) {
		var buf = ReadSquished(br, out int overreadCount);

		using var simplifiedTga = new MemoryStream(buf);
		using var stbr = new BinaryReader(simplifiedTga);

		var value = new Tga {
			ContainsAlpha = stbr.ReadUInt16(),
			Width = stbr.ReadUInt16(),
			Height = stbr.ReadUInt16(),
			OverreadCount = overreadCount,
		};

		value.PixelData = new TgaPixel[value.Width * value.Height];

		if (value.ContainsAlpha == 1) {
			for (int i = 0; i < value.Width * value.Height; i++) {
				var pix = stbr.ReadUInt16();

				value.PixelData[i] = new TgaPixel() {
					red = C4to8[pix & 0xF],
					green = C4to8[(pix >> 4) & 0xF],
					blue = C4to8[(pix >> 8) & 0xF],
					alpha = C4to8[(pix >> 12) & 0xF],
				};
			}
		} else {
			for (int i = 0; i < value.Width * value.Height; i++) {
				var pix = stbr.ReadUInt16();

				value.PixelData[i] = new TgaPixel() {
					red = C5to8[pix & 0x1F],
					green = C6to8[(pix >> 5) & 0x3F],
					blue = C5to8[(pix >> 11) & 0x1F],
					alpha = 0,
				};
			}
		}

		return value;
	}

	public byte[] Serialize() {
		using var tgaMemory = new MemoryStream();
		using var tgaBw = new BinaryWriter(tgaMemory);

		// length of image ID -- we don't generate it
		tgaBw.Write((byte)0);

		// Whether a color map is included
		// 0 - indicates that no color-map data is included with this image.
		// 1 - indicates that a color-map is included with this image.)
		tgaBw.Write((byte)0);

		// Compression and color types
		// 0- No Image Data Included
		// 1- Uncompressed, Color mapped image
		// 2- Uncompressed, True Color Image
		// 9- Run-length encoded, Color mapped image
		// 11- Run-Length encoded, Black and white image
		tgaBw.Write((byte)2);

		// ---- Color Map -- Not used, but must be written
		tgaBw.Write((short)0); // First Entry Index
		tgaBw.Write((short)0); // Color Map length
		tgaBw.Write((byte)0); // Color map entry size

		// ---- Image specification
		tgaBw.Write((short)0); // x-origin
		tgaBw.Write((short)0); // y-origin
		tgaBw.Write(this.Width);
		tgaBw.Write(this.Height);
		// 32 bit color or 24-bit + 8-bit alpha
		tgaBw.Write((byte)(this.ContainsAlpha == 0 ? 24 : 32)); // 32 or 24
		tgaBw.Write((byte)(1 << 5)); // Image Descriptor (??)

		for (int i = 0; i < this.PixelData.Length; i++) {
			tgaBw.Write(this.PixelData[i].red);
			tgaBw.Write(this.PixelData[i].green);
			tgaBw.Write(this.PixelData[i].blue);
			if (this.ContainsAlpha == 1)
				tgaBw.Write(this.PixelData[i].alpha);
		}

		return tgaMemory.ToArray();
	}
}
