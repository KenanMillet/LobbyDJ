using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
	public class ShowPropertyAttribute : StackableDecoratorAttribute
	{
		bool m_GUIenabled = false;
#if UNITY_EDITOR
		public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
		{
			m_GUIenabled = GUI.enabled;
			if (!IsVisible()) return visible;
			GUI.enabled = false;
			int propertyNameLength = property.displayName.Length - ("<" + ">k__Backing Field").Length;
			label.text = property.displayName.Substring(1, propertyNameLength);
			return visible;
		}

		public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = m_GUIenabled;
		}
#endif
	}
}