using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

public class NetworkObjectSpawner : MonoBehaviour
{
	public static NetworkObjectSpawner Instance { get; private set; }
	private NetworkManager Manager => NetworkManager.Instance;

	[Serializable]
	public enum PrefabType
	{
		NetworkAudioMixer,
		NetworkAudioProfile,
		NetworkAudioLowPassFilter
	}
	[Serializable]
	public class NetworkPrefabListStorage : SerializableDictionary.Storage<GameObject[]> { }
	[Serializable]
	public class NetworkPrefabContainer : SerializableDictionary<PrefabType, NetworkPrefabListStorage> { }
	[SerializeField]
	private NetworkPrefabContainer Prefabs = new NetworkPrefabContainer();

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		MainThreadManager.Create();
		DontDestroyOnLoad(gameObject);
		
		foreach(var kvp in Prefabs)
		{
			Manager.GetType()
				.GetField(kvp.Key.ToString() + "NetworkObject")
				.SetValue(Manager, kvp.Value.data);
		}
	}

	public NetworkBehavior Spawn(PrefabType prefabType, int index = 0, Vector3? position = null, Quaternion? rotation = null, bool sendTransform = true)
	{
		return Manager.GetType()
				.GetMethod("Instantiate" + prefabType.ToString())
				.Invoke(Manager, new object[] { index, position, rotation, sendTransform }) as NetworkBehavior;
	}
}