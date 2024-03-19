namespace AssetTools.UCFileStructures.Prim;

[Deserializer.DeserializeGenerator]
public partial class NPrim
{
	public ushort PointsStartId { get; set; }

	public ushort PointsEndId { get; set; }

	[Deserializer.Skip]
	public int PointsCount => this.PointsEndId - this.PointsStartId;


	public ushort Face4StartId { get; set; }

	public ushort Face4EndId { get; set; }

	[Deserializer.Skip]
	public int Faces4Count => this.Face4EndId - this.Face4StartId;

	public short Face3StartId { get; set; }

	public short Face3EndId { get; set; }

	[Deserializer.Skip]
	public int Faces3Count => this.Face3EndId - this.Face3StartId;

	/**
	 * #define PRIM_COLLIDE_BOX		0	// As a rotated bounding box
	 * #define PRIM_COLLIDE_NONE		1	// Just walk through the prim
	 * #define PRIM_COLLIDE_CYLINDER	2	// As a cylinder
	 * #define PRIM_COLLIDE_SMALLBOX	3	// A bounding box smaller than the prim
	 */
	public byte CollisionType { get; set; }
	/**
	 * How this prim gets damaged
	 * PRIM_DAMAGE_DAMAGABLE = (1 << 0)
	 * PRIM_DAMAGE_EXPLODES =  (1 << 1)
	 * PRIM_DAMAGE_CRUMPLE =   (1 << 2)
	 * PRIM_DAMAGE_LEAN =      (1 << 3)
	 * PRIM_DAMAGE_NOLOS =     (1 << 4) // You can't see through this prim (included in LOS calculation)
	 */
	public byte DamageType { get; set; }

	/**
	 * The type of shadow to draw under the prim.
	 * PRIM_SHADOW_NONE =     0
	 * PRIM_SHADOW_BOXEDGE =  1
	 * PRIM_SHADOW_CYLINDER = 2
	 * PRIM_SHADOW_FOURLEGS = 3
	 * PRIM_SHADOW_FULLBOX =  4
	 */
	public byte ShadowType { get; set; }

	/**
	 * Prim Flags
	 * PRIM_FLAG_LAMPOST =                 (1 << 0)
	 * PRIM_FLAG_CONTAINS_WALKABLE_FACES = (1 << 1)
	 * PRIM_FLAG_GLARE =                   (1 << 2)
	 * PRIM_FLAG_ITEM =                    (1 << 3)
	 * PRIM_FLAG_TREE =                    (1 << 4)
	 * PRIM_FLAG_ENVMAPPED =               (1 << 5)
	 * PRIM_FLAG_JUST_FLOOR =              (1 << 6)
	 * PRIM_FLAG_ON_FLOOR =                (1 << 7)
	 */
	public byte PrimFlags { get; set; }
}
