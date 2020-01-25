using UnityEngine;

[RequireComponent(typeof(ManagedRectTransform))]
public class ContentTransform : MonoBehaviour
{
	public RectTransform Top => _Top == null ? null : _Top.transform as RectTransform;
	public RectTransform Bottom => _Bottom == null ? null : _Bottom.transform as RectTransform;

	[SerializeField]
	private ManagedRectTransform _Top = null, _Bottom = null;
	private void Reset() => _Bottom = _Top = GetComponent<ManagedRectTransform>();
}