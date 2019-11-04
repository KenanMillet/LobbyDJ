using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkAudioProfile : NetworkAudioProfileBehavior
{
	public AudioSource Source { get; private set; } = null;

	HashSet<uint> AttachIds = new HashSet<uint>();

	public Action OnSourceReady = null;

	public override void FetchSource(RpcArgs args)
	{
		string uri = args.GetNext<string>();
		AudioType type = (AudioType)args.GetNext<int>();
		FetchSource(uri, type);
	}
	public void FetchSource(string uri, AudioType type)
	{
		Debug.Log("Fetching source (" + type.ToString() + "): " + uri);
		AudioSource src = gameObject.AddComponent<AudioSource>();
		StartCoroutine(GetAudioClip(src, uri, type));
	}

	public override void Attach(RpcArgs args)
	{
		uint netId = args.GetNext<uint>();
		Attach(netId);
	}
	public void Attach(uint netId)
	{
		AttachIds.Add(netId);
	}
	public void Attach(NetworkBehavior attachment)
	{
		attachment.transform.parent = transform;
	}

	public override void Detach(RpcArgs args)
	{
		uint netId = args.GetNext<uint>();
		Detach(netId);
	}
	public void Detach(uint netId)
	{
		Debug.Log("Detaching object: " + netId);
		NetworkBehavior attachment = networkObject.Networker.NetworkObjects[netId].AttachedBehavior as NetworkBehavior;
		Detach(attachment);
	}
	public void Detach(NetworkBehavior attachment)
	{
		if (attachment.transform.IsChildOf(transform))
		{
			attachment.transform.parent = null;
		}
	}

	IEnumerator GetAudioClip(AudioSource source, string uri, AudioType type)
	{
		OnSourceReady = null;
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
		OnSourceReady?.Invoke();
	}

	public override void Play(RpcArgs args)
	{
		int timeSamples = args.GetNext<int>();
		Play(timeSamples);
	}
	public void Play(int timeSamples)
	{
		if (Source != null)
		{
			Debug.Log("Playing at time: " + timeSamples);
			Source.timeSamples = timeSamples;
			Source.Play();
		}
		else
		{
			float time = Time.time;
			OnSourceReady += () =>
			{
				Play(timeSamples);
				Source.time += Time.time - time;
			};
		}
	}

	public override void Pause(RpcArgs args)
	{
		Pause();
	}
	public void Pause()
	{
		if (Source != null)
		{
			Debug.Log("Pausing...");
			Source.Pause();
		}
		else
		{
			OnSourceReady += () => Pause();
		}
	}

	private void Update()
	{
		foreach(uint netId in AttachIds)
		{
			if (networkObject.Networker.NetworkObjects.TryGetValue(netId, out NetworkObject obj))
			{
				Debug.Log("Attaching object: " + netId);
				Attach(obj.AttachedBehavior as NetworkBehavior);
				AttachIds.Remove(netId);
				break;
			}
		}
	}
}
