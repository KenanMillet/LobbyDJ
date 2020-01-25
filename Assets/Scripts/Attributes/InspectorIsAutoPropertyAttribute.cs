using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
	public class IsAutoPropertyAttribute : StackableDecoratorAttribute
	{
#if UNITY_EDITOR
		public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
		{
			if (!IsVisible()) return visible;

			const string propertyPrefix = "<";
			const string propertySuffix = ">k__Backing Field";

			if (property.displayName.StartsWith(propertyPrefix) && property.displayName.EndsWith(propertySuffix))
			{
				int propertyNameLength = property.displayName.Length - (propertyPrefix + propertySuffix).Length;
				label.text = property.displayName.Substring(1, propertyNameLength);
			}
			else label.text = property.displayName;

			return visible;
		}
#endif
	}
}