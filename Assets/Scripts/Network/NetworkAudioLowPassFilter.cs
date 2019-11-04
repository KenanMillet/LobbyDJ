using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class NetworkAudioLowPassFilter : NetworkAudioLowPassFilterBehavior
{
	AudioLowPassFilter Filter = null;

	private void OnBeforeTransformParentChanged()
	{
		if (transform.parent != null)
		{
			Destroy(Filter);
			Filter = null;
		}
	}

	private void OnTransformParentChanged()
	{
		if (transform.parent != null)
		{
			Filter = transform.parent.gameObject.AddComponent<AudioLowPassFilter>();
		}
	}

	// Update is called once per frame
	void Update()
    {
        if (Filter != null)
		{
			Filter.cutoffFrequency = networkObject.CutoffFrequency;
			Filter.lowpassResonanceQ = networkObject.LowpassResonanceQ;
		}
    }
}
