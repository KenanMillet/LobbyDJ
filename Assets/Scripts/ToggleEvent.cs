using System;
using UnityEngine;
using UnityEngine.Events;

public class ToggleEvent : MonoBehaviour
{
	[SerializeField]
	private bool value = false;
	
	public UnityEvent OnTrue;
	public UnityEvent OnFalse;

	[Serializable]
	public class OnToggleEvent : UnityEvent<bool> { }
	public OnToggleEvent OnToggle;

	public void Toggle()
	{
		value = !value;
		OnToggle?.Invoke(value);
		OnValidate();
	}

	private void OnValidate()
	{
		if (value) OnTrue?.Invoke();
		else OnFalse?.Invoke();
	}
}
