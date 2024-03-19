using System;
using System.IO;

namespace AssetTools.UCFileStructures;

[Deserializer.DeserializeGenerator]
public partial class FileClump
{

	public uint MaxID { get; set; }

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(MaxID))]
	public int[] Offsets { get; set; } // [MaxID] -- Officially, size_t[] but Urban Chaos is 32bits, so this must be int

	[Deserializer.VariableSizedArray(SizePropertyName = nameof(MaxID))]
	public int[] Lenghts { get; set; } // [MaxID] -- Officially, size_t[] but Urban Chaos is 32bits, so this must be int

	[Deserializer.DeserializeFn(FnName = nameof(DeserializeFiles))]
	public byte[][] Files { get; set; }

	public byte[] GetFile(int id) {
		if (id < 0 || id > this.Files.Length) {
			throw new Exception($"File {id} is out of range (0 --> {this.Files.Length})");
		}

		return this.Files[id];
	}

	private static byte[][] DeserializeFiles(FileClump value, BinaryReader br) {
		var values = new byte[value.Offsets.Length][];

		for (int i = 0; i < value.Offsets.Length; i++) {
			int offset = value.Offsets[i];
			int length = value.Lenghts[i];

			if (offset == 0 || length == 0) {
				continue;
			}

			_ = br.BaseStream.Seek(offset, SeekOrigin.Begin);
			values[i] = br.ReadBytes(length);
		}

		return values;
	}
}
