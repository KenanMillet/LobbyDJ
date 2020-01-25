using StackableDecorator;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(ContentTransform))]
public class DynamicContentList : MonoBehaviour
{
	public enum PaddingDirection { Down, Right };

	[field: SerializeField, IsAutoProperty, StackableField]
	public float Padding { get; set; } = 0;
	[field: SerializeField, IsAutoProperty, StackableField]
	public PaddingDirection PaddingDir { get; set; } = PaddingDirection.Down;
	private float OldPadding = 0;
	private PaddingDirection OldPaddingDir = PaddingDirection.Down;

	private Vector2 PaddingV => (PaddingDir == PaddingDirection.Down ? Vector2.down : Vector2.right) * Padding;

	[SerializeField, StackableField]
	private List<ContentTransform> Contents = new List<ContentTransform>();

	private void Start()
	{
		ContentTransform ct = GetComponent<ContentTransform>();
		ct.Top.pivot = new Vector2(0, 1);
		ct.Top.anchorMin = new Vector2(0, 1);
		ct.Top.anchorMax = ct.Top.anchorMin;

		OldPadding = Padding;
		OldPaddingDir = PaddingDir;

		List<ContentTransform> initContents = Contents;
		Contents = new List<ContentTransform>();
		Add(initContents);
	}

	private void LateUpdate()
	{
		Canvas.ForceUpdateCanvases();
		if ((OldPadding != Padding || OldPaddingDir != PaddingDir) && Contents.Count > 0)
		{
			foreach (ContentTransform content in Contents.Skip(1))
			{
				content.Top.anchorMin = PaddingDir == PaddingDirection.Down ? Vector2.zero : Vector2.one;
				content.Top.anchorMax = content.Top.anchorMin;
				content.Top.anchoredPosition = PaddingV;
			}
			ResizeToFitContent();
		}
	}

	public void Add(IEnumerable<ContentTransform> contents)
	{
		foreach (ContentTransform content in contents)
		{
			AddContentTransform(content);
		}
		ResizeToFitContent();
	}

	public void Add(ContentTransform content)
	{
		AddContentTransform(content);
		ResizeToFitContent();
	}

	private void AddContentTransform(ContentTransform content)
	{
		content.Top.pivot = new Vector2(0, 1);
		if (Contents.Count == 0)
		{
			content.Top.SetParent(GetComponent<ContentTransform>().Top);
			content.Top.anchorMin = new Vector2(0, 1);
			content.Top.anchoredPosition = Vector2.zero;
		}
		else
		{
			content.Top.SetParent(Contents[Contents.Count - 1].Bottom);
			content.Top.anchorMin = PaddingDir == PaddingDirection.Down ? Vector2.zero : Vector2.one;
			content.Top.anchoredPosition = PaddingV;
		}
		content.Top.anchorMax = content.Top.anchorMin;
		Contents.Add(content);
	}

	public void ResizeToFitContent()
	{	
		if (Contents.Count > 0)
		{
			Bounds contentBounds = Contents[0].Top.GetWorldBounds();
			if (Contents[0].Top != Contents[0].Bottom)
			{
				contentBounds.Encapsulate(Contents[0].Bottom.GetWorldBounds());
			}
			foreach (ContentTransform content in Contents.Skip(1))
			{
				contentBounds.Encapsulate(content.Top.GetWorldBounds());
				if (content.Top != content.Bottom)
				{
					contentBounds.Encapsulate(content.Bottom.GetWorldBounds());
				}
			}
			GetComponent<ContentTransform>().Top.sizeDelta = Vector2.Scale(contentBounds.size, transform.lossyScale.Inverse());
		}
	}
}