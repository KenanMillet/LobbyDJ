using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using StackableDecorator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkAudioProfile : NetworkAudioProfileBehavior
{
	public AudioSource Source { get; private set; } = null;
	public NetworkingPlayer MyPlayer => networkObject.Networker.GetPlayerById(networkObject.MyPlayerId);

	[field: SerializeField, IsAutoProperty, Disable, StackableField]
	public string URI { get; private set; } = null;
	[field: SerializeField, IsAutoProperty, Disable, StackableField]
	public AudioType AudioFormat { get; private set; } = AudioType.UNKNOWN;

	HashSet<uint> ToAttachIds = new HashSet<uint>();
	HashSet<uint> AttachedIds = new HashSet<uint>();

	public Action OnSourceReady = null;





	private void Start()
	{
		if (networkObject.IsServer)
		{
			networkObject.Networker.playerAccepted += SyncPlayer;
		}
	}

	private void Update()
	{
		foreach (uint netId in ToAttachIds)
		{
			if (networkObject.Networker.NetworkObjects.TryGetValue(netId, out NetworkObject obj))
			{
				Debug.Log("Attaching object: " + netId);
				NetworkBehavior attachment = obj.AttachedBehavior as NetworkBehavior;
				attachment.transform.parent = transform;
				AttachedIds.Add(netId);
				ToAttachIds.Remove(netId);
				break;
			}
		}
	}

	private void SyncPlayer(NetworkingPlayer player, NetWorker sender)
	{
		if (Source != null)
		{
			networkObject.SendRpc(player, RPC_FETCH_SOURCE, URI, (int)AudioFormat);
			if (Source.isPlaying)
			{
				networkObject.SendRpc(player, RPC_PLAY, Source.timeSamples);
			}
		}
		foreach (uint netId in AttachedIds)
		{
			networkObject.SendRpc(player, RPC_ATTACH, netId);
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
		OnSourceReady?.Invoke();
		OnSourceReady = null;
	}





	public override void FetchSource(RpcArgs args)
	{
		string uri = args.GetNext<string>();
		AudioType type = (AudioType)args.GetNext<int>();
		Destroy(Source);
		Source = null;
		Debug.Log("Fetching source (" + type.ToString() + "): " + uri);
		AudioSource src = gameObject.AddComponent<AudioSource>();
		src.hideFlags = HideFlags.HideInInspector;
		URI = uri;
		AudioFormat = type;
		StartCoroutine(GetAudioClip(src, uri, type));
	}
	public void FetchSource(string uri, AudioType type, NetworkingPlayer player = null)
	{
		if (player == null)
		{
			networkObject.SendRpc(RPC_FETCH_SOURCE, Receivers.All, uri, (int)type);
			player = MyPlayer;
		}
		networkObject.SendRpc(player, RPC_FETCH_SOURCE, uri, (int)type);
	}

	public override void Attach(RpcArgs args)
	{
		uint netId = args.GetNext<uint>();
		ToAttachIds.Add(netId);
	}
	public void Attach(uint netId, NetworkingPlayer player = null)
	{
		if (player == null)
		{
			networkObject.SendRpc(RPC_ATTACH, Receivers.All, netId);
			player = MyPlayer;
		}
		networkObject.SendRpc(player, RPC_ATTACH, netId);
	}

	public override void Detach(RpcArgs args)
	{
		uint netId = args.GetNext<uint>();
		Debug.Log("Detaching object: " + netId);
		NetworkBehavior attachment = networkObject.Networker.NetworkObjects[netId].AttachedBehavior as NetworkBehavior;
		if (attachment.transform.IsChildOf(transform))
		{
			attachment.transform.parent = null;
		}
		AttachedIds.Remove(netId);
	}
	public void Detach(uint netId, NetworkingPlayer player = null)
	{
		if (player == null)
		{
			networkObject.SendRpc(RPC_DETACH, Receivers.All, netId);
			player = MyPlayer;
		}
		networkObject.SendRpc(player, RPC_DETACH, netId);
	}

	public override void Play(RpcArgs args)
	{
		if (Source != null)
		{
			int timeSamples = args.GetNext<int>();
			Debug.Log("Playing at time: " + timeSamples);
			Source.timeSamples = timeSamples;
			Source.Play();
		}
		else
		{
			Debug.Log("Will Play when Source is ready...");
			float time = Time.time;
			OnSourceReady += () =>
			{
				Play(args);
				Source.time += Time.time - time;
			};
		}
	}
	public void Play(int timeSamples, NetworkingPlayer player = null)
	{
		if (player == null)
		{
			networkObject.SendRpc(RPC_PLAY, Receivers.All, timeSamples);
			player = MyPlayer;
		}
		networkObject.SendRpc(player, RPC_PLAY, timeSamples);
	}

	public override void Pause(RpcArgs args)
	{
		if (Source != null)
		{
			Debug.Log("Pausing...");
			Source.Pause();
		}
		else
		{
			Debug.Log("Will Pause when Source is ready...");
			OnSourceReady += () => Pause(args);
		}
	}
	public void Pause(NetworkingPlayer player = null)
	{
		if (player == null)
		{
			networkObject.SendRpc(RPC_PAUSE, Receivers.All);
			player = MyPlayer;
		}
		networkObject.SendRpc(player, RPC_PAUSE);
	}
}
