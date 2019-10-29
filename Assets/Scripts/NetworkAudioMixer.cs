using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class NetworkAudioMixer : NetworkAudioMixerBehavior
{
	AudioSource Source => transform.parent?.GetComponent<NetworkAudioProfile>()?.Source;
	private void Update()
	{
		if (Source != null)
		{
			Source.volume = networkObject.volume;
		}
	}
}
