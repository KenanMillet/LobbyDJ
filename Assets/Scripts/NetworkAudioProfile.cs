using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkAudioProfile : NetworkAudioProfileBehavior
{
	public AudioSource Source { get; private set; } = null;

	public override void FetchSource(RpcArgs args)
	{
		string uri = args.GetNext<string>();
		AudioType type = (AudioType)args.GetNext<int>();
		AudioSource src = gameObject.AddComponent<AudioSource>();
		StartCoroutine(GetAudioClip(src, uri, type));
	}

	public override void Attach(RpcArgs args)
	{
		uint netId = args.GetNext<uint>();
		NetworkBehavior attachment = networkObject.Networker.NetworkObjects[netId].AttachedBehavior as NetworkBehavior;
		attachment.transform.parent = transform;
	}

	public override void Detach(RpcArgs args)
	{
		uint netId = args.GetNext<uint>();
		NetworkBehavior attachment = networkObject.Networker.NetworkObjects[netId].AttachedBehavior as NetworkBehavior;
		if (attachment.transform.IsChildOf(transform))
		{
			attachment.transform.parent = null;
		}
	}

	IEnumerator GetAudioClip(AudioSource source, string uri, AudioType type)
	{
		using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, type))
		{
			yield return www.SendWebRequest();

			if (www.isNetworkError)
			{
				Debug.Log(www.error);
			}
			else
			{
				source.clip = DownloadHandlerAudioClip.GetContent(www);
				Source = source;
			}
		}
	}
}
