namespace AssetTools.UCWorld.Utils;

public class MathUtils
{
	// While C# has its own Math.PI, let's use the same one used in UC source
	public const float PI = 3.14159265F;

	public static int QDist2(int x, int y) {
		if (x > y) {
			return x + (y / 2);
		}

		return y + (x / 2);
	}
}
