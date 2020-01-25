using UnityEngine;

public static class RectTransformExtensions
{
	public static Bounds GetWorldBounds(this RectTransform rt)
	{
		Vector3[] corners = new Vector3[4];
		rt.GetWorldCorners(corners);
		Vector3 center = (corners[0] + corners[2]) / 2;
		Vector3 size = new Vector3
		{
			x = Mathf.Abs(corners[2].x - corners[0].x),
			y = Mathf.Abs(corners[2].y - corners[0].y),
			z = Mathf.Abs(corners[2].z - corners[0].z),
		};
		return new Bounds(center, size);
	}
}
