using UnityEngine;

[RequireComponent(typeof(ContentTransform))]
public class AudioProfileDrawer : MonoBehaviour
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

	public void Play()
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
			Profile.Play(Profile.Source.timeSamples);
		}
	}

	public void Pause()
	{
		if(Profile?.Source != null)
		{
			Profile.Pause();
		}
	}

	private void Update()
	{
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
