using StackableDecorator;
using UnityEngine;

[RequireComponent(typeof(ManagedRectTransform))]
public class AudioProfileDetailsDrawer : MonoBehaviour
{
	[field: SerializeField, IsAutoProperty, StackableField]
	public float CollapsedHeight { get; private set; } = 0;
	[field: SerializeField, IsAutoProperty, StackableField]
	public float ExpandedHeight { get; private set; } = 0;

	public void Collapse()
	{
		(transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CollapsedHeight);
	}

	public void Expand()
	{
		(transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ExpandedHeight);
	}
}
