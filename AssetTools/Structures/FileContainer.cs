namespace AssetTools.Structures;

/// <summary>
/// Stores details about the underlying file being read.
/// </summary>
/// <typeparam name="T"></typeparam>
public class FileContainer<T>
{
	public T Data { get; set; }
	public string Path { get; set; }
	public AssetLoadStatus Status { get; set; }

	public FileContainer(T data, string path, AssetLoadStatus status) {
		this.Data = data;
		this.Path = path;
		this.Status = status;
	}
}
