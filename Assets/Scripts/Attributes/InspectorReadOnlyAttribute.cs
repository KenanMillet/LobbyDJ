using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
	public class ReadOnlyAttribute : StackableDecoratorAttribute
	{
		bool m_GUIenabled = false;
#if UNITY_EDITOR
		public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
		{
			m_GUIenabled = GUI.enabled;
			if (!IsVisible()) return visible;
			GUI.enabled = false;
			return visible;
		}

		public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = m_GUIenabled;
		}
#endif
	}
}