using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Transform))]
public class ManagedTransform : MonoBehaviour
{
	private void LateUpdate()
	{
		transform.hasChanged = false;
	}
}