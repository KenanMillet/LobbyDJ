using StackableDecorator;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicContentList : MonoBehaviour
{
	[field: SerializeField, IsAutoProperty, NotNull("Content Container should not be null"), StackableField]
	public RectTransform ContentContainer { get; private set; } = null;

	public enum EOrientation { TopToBottom, LeftToRight };

	[field: Space(10)]
	[field: SerializeField, IsAutoProperty, StackableField]
	public float TopMargin { get; private set; } = 0;
	[field: SerializeField, IsAutoProperty, StackableField]
	public float BottomMargin { get; private set; } = 0;
	[field: SerializeField, IsAutoProperty, StackableField]
	public float LeftMargin { get; private set; } = 0;
	[field: SerializeField, IsAutoProperty, StackableField]
	public float RightMargin { get; private set; } = 0;
	private float OldTopMargin = 0, OldBottomMargin = 0, OldLeftMargin = 0, OldRightMargin = 0;

	private Vector2 OpenMarginV => new Vector2(LeftMargin, TopMargin);
	private Vector2 CloseMarginV => new Vector2(RightMargin, BottomMargin);

	[field:Space(10)]
	[field: SerializeField, IsAutoProperty, StackableField]
	public float Padding { get; private set; } = 0;
	[field: SerializeField, IsAutoProperty, StackableField]
	public EOrientation Orientation { get; private set; } = EOrientation.TopToBottom;
	private float OldPadding = 0;
	private EOrientation OldOrientation = EOrientation.TopToBottom;

	private Vector2 PaddingV => (Orientation == EOrientation.TopToBottom ? Vector2.down : Vector2.right) * Padding;

	[SerializeField, StackableField]
	private List<ContentTransform> Contents = new List<ContentTransform>();

	private void Start()
	{
		List<ContentTransform> initContents = Contents;
		Contents = new List<ContentTransform>();
		Add(initContents);
	}

	private void LateUpdate()
	{
		Canvas.ForceUpdateCanvases();
		bool shouldResize = false;
		if (Contents.Exists(ct => ct.Top.hasChanged || ct.Bottom.hasChanged))
		{
			shouldResize = true;
		}
		if ((OldTopMargin != TopMargin || OldLeftMargin != LeftMargin) && Contents.Count > 0)
		{
			Contents[0].Top.anchoredPosition = Vector2.Scale(Vector2.down + Vector2.right, OpenMarginV);
			shouldResize = true;
		}
		if ((OldBottomMargin != BottomMargin || OldRightMargin != RightMargin) && Contents.Count > 0)
		{
			shouldResize = true;
		}
		if ((OldPadding != Padding || OldOrientation != Orientation) && Contents.Count > 0)
		{
			foreach (ContentTransform content in Contents.Skip(1))
			{
				content.Top.anchorMin = Orientation == EOrientation.TopToBottom ? Vector2.zero : Vector2.one;
				content.Top.anchorMax = content.Top.anchorMin;
				content.Top.anchoredPosition = PaddingV;
			}
			shouldResize = true;
		}


		if (shouldResize)
		{
			ResizeToFitContent();
		}

		OldTopMargin = TopMargin;
		OldBottomMargin = BottomMargin;
		OldLeftMargin = LeftMargin;
		OldRightMargin = RightMargin;

		OldPadding = Padding;
		OldOrientation = Orientation;
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
			content.Top.SetParent(ContentContainer);
			content.Top.anchorMin = new Vector2(0, 1);
			content.Top.anchoredPosition = Vector2.Scale(Vector2.down + Vector2.right, OpenMarginV);
		}
		else
		{
			content.Top.SetParent(Contents[Contents.Count - 1].Bottom);
			content.Top.anchorMin = Orientation == EOrientation.TopToBottom ? Vector2.zero : Vector2.one;
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
			ContentContainer.sizeDelta = Vector2.Scale(contentBounds.size, transform.lossyScale.Inverse()) + OpenMarginV + CloseMarginV;
		}
	}
}