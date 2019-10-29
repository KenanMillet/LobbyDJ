using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0.15]")]
	public partial class NetworkAudioMixerNetworkObject : NetworkObject
	{
		public const int IDENTITY = 4;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		private float _volume;
		public event FieldEvent<float> volumeChanged;
		public InterpolateFloat volumeInterpolation = new InterpolateFloat() { LerpT = 0.15f, Enabled = true };
		public float volume
		{
			get { return _volume; }
			set
			{
				// Don't do anything if the value is the same
				if (_volume == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_volume = value;
				hasDirtyFields = true;
			}
		}

		public void SetvolumeDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_volume(ulong timestep)
		{
			if (volumeChanged != null) volumeChanged(_volume, timestep);
			if (fieldAltered != null) fieldAltered("volume", _volume, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			volumeInterpolation.current = volumeInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _volume);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_volume = UnityObjectMapper.Instance.Map<float>(payload);
			volumeInterpolation.current = _volume;
			volumeInterpolation.target = _volume;
			RunChange_volume(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _volume);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (volumeInterpolation.Enabled)
				{
					volumeInterpolation.target = UnityObjectMapper.Instance.Map<float>(data);
					volumeInterpolation.Timestep = timestep;
				}
				else
				{
					_volume = UnityObjectMapper.Instance.Map<float>(data);
					RunChange_volume(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (volumeInterpolation.Enabled && !volumeInterpolation.current.UnityNear(volumeInterpolation.target, 0.0015f))
			{
				_volume = (float)volumeInterpolation.Interpolate();
				//RunChange_volume(volumeInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public NetworkAudioMixerNetworkObject() : base() { Initialize(); }
		public NetworkAudioMixerNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public NetworkAudioMixerNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
