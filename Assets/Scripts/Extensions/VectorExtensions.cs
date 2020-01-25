using UnityEngine;

public static class VectorExtensions
{
	public static Vector4 Inverse(this Vector4 v) => new Vector4(1f / v.x, 1f / v.y, 1f / v.z, 1f/v.w);
	public static Vector3 Inverse(this Vector3 v) => new Vector3(1f / v.x, 1f / v.y, 1f / v.z);
	public static Vector2 Inverse(this Vector2 v) => new Vector2(1f / v.x, 1f / v.y);
}
