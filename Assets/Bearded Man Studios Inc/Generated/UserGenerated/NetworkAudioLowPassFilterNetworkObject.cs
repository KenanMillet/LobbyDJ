using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0.15,0.15]")]
	public partial class NetworkAudioLowPassFilterNetworkObject : NetworkObject
	{
		public const int IDENTITY = 8;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		private float _CutoffFrequency;
		public event FieldEvent<float> CutoffFrequencyChanged;
		public InterpolateFloat CutoffFrequencyInterpolation = new InterpolateFloat() { LerpT = 0.15f, Enabled = true };
		public float CutoffFrequency
		{
			get { return _CutoffFrequency; }
			set
			{
				// Don't do anything if the value is the same
				if (_CutoffFrequency == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_CutoffFrequency = value;
				hasDirtyFields = true;
			}
		}

		public void SetCutoffFrequencyDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_CutoffFrequency(ulong timestep)
		{
			if (CutoffFrequencyChanged != null) CutoffFrequencyChanged(_CutoffFrequency, timestep);
			if (fieldAltered != null) fieldAltered("CutoffFrequency", _CutoffFrequency, timestep);
		}
		private float _LowpassResonanceQ;
		public event FieldEvent<float> LowpassResonanceQChanged;
		public InterpolateFloat LowpassResonanceQInterpolation = new InterpolateFloat() { LerpT = 0.15f, Enabled = true };
		public float LowpassResonanceQ
		{
			get { return _LowpassResonanceQ; }
			set
			{
				// Don't do anything if the value is the same
				if (_LowpassResonanceQ == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_LowpassResonanceQ = value;
				hasDirtyFields = true;
			}
		}

		public void SetLowpassResonanceQDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_LowpassResonanceQ(ulong timestep)
		{
			if (LowpassResonanceQChanged != null) LowpassResonanceQChanged(_LowpassResonanceQ, timestep);
			if (fieldAltered != null) fieldAltered("LowpassResonanceQ", _LowpassResonanceQ, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			CutoffFrequencyInterpolation.current = CutoffFrequencyInterpolation.target;
			LowpassResonanceQInterpolation.current = LowpassResonanceQInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _CutoffFrequency);
			UnityObjectMapper.Instance.MapBytes(data, _LowpassResonanceQ);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_CutoffFrequency = UnityObjectMapper.Instance.Map<float>(payload);
			CutoffFrequencyInterpolation.current = _CutoffFrequency;
			CutoffFrequencyInterpolation.target = _CutoffFrequency;
			RunChange_CutoffFrequency(timestep);
			_LowpassResonanceQ = UnityObjectMapper.Instance.Map<float>(payload);
			LowpassResonanceQInterpolation.current = _LowpassResonanceQ;
			LowpassResonanceQInterpolation.target = _LowpassResonanceQ;
			RunChange_LowpassResonanceQ(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _CutoffFrequency);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _LowpassResonanceQ);

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
				if (CutoffFrequencyInterpolation.Enabled)
				{
					CutoffFrequencyInterpolation.target = UnityObjectMapper.Instance.Map<float>(data);
					CutoffFrequencyInterpolation.Timestep = timestep;
				}
				else
				{
					_CutoffFrequency = UnityObjectMapper.Instance.Map<float>(data);
					RunChange_CutoffFrequency(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (LowpassResonanceQInterpolation.Enabled)
				{
					LowpassResonanceQInterpolation.target = UnityObjectMapper.Instance.Map<float>(data);
					LowpassResonanceQInterpolation.Timestep = timestep;
				}
				else
				{
					_LowpassResonanceQ = UnityObjectMapper.Instance.Map<float>(data);
					RunChange_LowpassResonanceQ(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (CutoffFrequencyInterpolation.Enabled && !CutoffFrequencyInterpolation.current.UnityNear(CutoffFrequencyInterpolation.target, 0.0015f))
			{
				_CutoffFrequency = (float)CutoffFrequencyInterpolation.Interpolate();
				//RunChange_CutoffFrequency(CutoffFrequencyInterpolation.Timestep);
			}
			if (LowpassResonanceQInterpolation.Enabled && !LowpassResonanceQInterpolation.current.UnityNear(LowpassResonanceQInterpolation.target, 0.0015f))
			{
				_LowpassResonanceQ = (float)LowpassResonanceQInterpolation.Interpolate();
				//RunChange_LowpassResonanceQ(LowpassResonanceQInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public NetworkAudioLowPassFilterNetworkObject() : base() { Initialize(); }
		public NetworkAudioLowPassFilterNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public NetworkAudioLowPassFilterNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
