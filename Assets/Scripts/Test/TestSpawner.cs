using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;


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
				Profile.networkObject.SendRpc(NetworkAudioProfileBehavior.RPC_FETCH_SOURCE, Receivers.All, Uri, (int)Type);
				Profile.FetchSource(Uri, Type);
				Profile.OnSourceReady += () =>
				{
					Profile.networkObject.SendRpc(NetworkAudioProfileBehavior.RPC_PLAY, Receivers.All, Profile.Source.timeSamples);
					Profile.Play(Profile.Source.timeSamples);
				};

				if (Mixer == null)
				{
					Mixer = NetworkObjectSpawner.Instance.Spawn(NetworkObjectSpawner.PrefabType.NetworkAudioMixer) as NetworkAudioMixer;
					Profile.networkObject.SendRpc(NetworkAudioProfileBehavior.RPC_ATTACH, Receivers.All, Mixer.networkObject.NetworkId);
					Profile.Attach(Mixer);
				}
			}
			else if (Profile?.Source != null)
			{
				if(Profile.Source.isPlaying)
				{
					Profile.networkObject.SendRpc(NetworkAudioProfileBehavior.RPC_PAUSE, Receivers.All);
					Profile.Pause();
				}
				else
				{
					Profile.networkObject.SendRpc(NetworkAudioProfileBehavior.RPC_PLAY, Receivers.All, Profile.Source.timeSamples);
					Profile.Play(Profile.Source.timeSamples);
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.L))
		{
			if (LPFilter == null)
			{
				LPFilter = NetworkObjectSpawner.Instance.Spawn(NetworkObjectSpawner.PrefabType.NetworkAudioLowPassFilter) as NetworkAudioLowPassFilter;
				Profile?.networkObject.SendRpc(NetworkAudioProfileBehavior.RPC_ATTACH, Receivers.All, LPFilter.networkObject.NetworkId);
				Profile?.Attach(LPFilter);
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
