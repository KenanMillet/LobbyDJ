﻿using UnityEngine;


public class TestSpawner : MonoBehaviour
{
	public string Uri;
	public AudioType Type;

	[Range(0.0f, 1.0f)]
	public float volume = 1.0f;

	[Range(10.0f, 22000.0f)]
	public float LPCutoffFreq = 5007.7f;
	public float LPResonanceQ = 1.0f;

	private NetworkAudioProfile Profile = null;
	private NetworkAudioMixer Mixer = null;
	private NetworkAudioLowPassFilter LPFilter = null;

	private void Start()
	{
		Uri = "File:///" + Uri;
	}

	// Update is called once per frame
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
		{
			if (Profile == null)
			{
				Profile = NetworkObjectSpawner.Instance.Spawn(NetworkObjectSpawner.PrefabType.NetworkAudioProfile) as NetworkAudioProfile;
				Profile.OnSourceReady += () =>
				{
					Profile.Play(Profile.Source.timeSamples);
				};
				Profile.FetchSource(Uri, Type);
				

				if (Mixer == null)
				{
					Mixer = NetworkObjectSpawner.Instance.Spawn(NetworkObjectSpawner.PrefabType.NetworkAudioMixer) as NetworkAudioMixer;
					Profile.Attach(Mixer.networkObject.NetworkId);
				}
			}
			else if (Profile?.Source != null)
			{
				if(Profile.Source.isPlaying)
				{
					Profile.Pause();
				}
				else
				{
					Profile.Play(Profile.Source.timeSamples);
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.L))
		{
			if (LPFilter == null)
			{
				LPFilter = NetworkObjectSpawner.Instance.Spawn(NetworkObjectSpawner.PrefabType.NetworkAudioLowPassFilter) as NetworkAudioLowPassFilter;
				Profile?.Attach(LPFilter.networkObject.NetworkId);
			}
		}

		if (Input.GetKey(KeyCode.DownArrow))
		{
			volume = Mathf.Max(volume - Time.deltaTime / 2.0f, 0.0f);
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{
			volume = Mathf.Min(volume + Time.deltaTime / 2.0f, 1.0f);
		}

		if (Mixer != null)
		{
			Mixer.networkObject.volume = volume;
		}
		if (LPFilter != null)
		{
			LPFilter.networkObject.CutoffFrequency = LPCutoffFreq;
			LPFilter.networkObject.LowpassResonanceQ = LPResonanceQ;
		}
    }
}
