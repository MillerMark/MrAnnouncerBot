using Bounce.Unmanaged;
using Photon;
using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace TaleSpireExploreDecompiles
{
	public class Ruler : PunBehaviour, IPunObservable
	{
		// Fields
		private const int MAX_HANDLE_COUNT = 10;
		private const int MAX_HANDLE_INDEX = 9;
		private const int SHARED_HANDLE_COUNT = 2;
		private const byte GM_ONLY_MASK = 0x40;
		[SerializeField]
		private RulerIndicator[] RulerIndicators;
		private Transform _transform;
		private PhotonView _photonView;
		private bool _remoteSync;
		private Values _values;
		private readonly float3[] _targetValues = new float3[10];
		private RulerIndicator _indicator;
		private NativeList<byte> _tmpBuffer;
		private bool _gmOnlyMode;
		private bool _disposed;

		// Methods
		private void DiscardHandle()
		{
			_values.Discard();
			SetNewIndicator(CurrentIndicatorIndex, true);
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				if (_tmpBuffer.IsCreated)
				{
					_tmpBuffer.Dispose();
					_tmpBuffer = new NativeList<byte>();
				}
				if (_photonView == null)
				{
					Destroy(base.gameObject);
				}
				else if (_photonView.isMine)
				{
					PhotonNetwork.Destroy(base.gameObject);
				}
			}
		}

		public bool GimzoAllowsReturnToEditting(int gizmoIndex)
		{
			int num = _values.ActiveHandle(CurrentIndicatorIndex);
			return (_indicator.SupportsReturningToEdit && ((_indicator.GizmoIndexToHandleIndex(gizmoIndex) == num) && (num < 9)));
		}

		public int HandleGizmoHover(Ray cameraRay)
		{
			if (_indicator != null)
			{
				return _indicator.HandleGizmoHover(cameraRay);
			}
			RulerIndicator local1 = _indicator;
			return -1;
		}

		private void Init(int startingMode)
		{
			_tmpBuffer = new NativeList<byte>(0x100, Allocator.Persistent);
			_transform = base.gameObject.transform;
			_values = new Values(RulerIndicators.Length);
			if (RulerIndicators.Length != 0)
			{
				SetNewIndicator(startingMode, false);
			}
		}

		public void NextHandle()
		{
			_values.Next(CurrentIndicatorIndex);
			_indicator.InitHandle(_values.ActiveHandle(CurrentIndicatorIndex));
		}

		public void NextMode(int offset = 1)
		{
			int length = RulerIndicators.Length;
			if (length > 1)
			{
				SetNewIndicator(Mathfx.TrueMod(CurrentIndicatorIndex + offset, length), false);
			}
		}

		public RulerBoardTool.ClickResult OnClick(int buttonId)
		{
			int index = _values.ActiveHandle(CurrentIndicatorIndex);
			switch (_indicator.OnClick(index, buttonId))
			{
				case ClickResponse.Cancel:
					return RulerBoardTool.ClickResult.Cancel;

				case ClickResponse.DiscardClick:
					return RulerBoardTool.ClickResult.Ignore;

				case ClickResponse.NextHandle:
					if (index == 9)
					{
						return RulerBoardTool.ClickResult.Finish;
					}
					NextHandle();
					return RulerBoardTool.ClickResult.Ignore;

				case ClickResponse.DiscardHandle:
					if (index > 0)
					{
						DiscardHandle();
					}
					return RulerBoardTool.ClickResult.Ignore;

				case ClickResponse.DiscardHandleAndFinish:
					if (index > 0)
					{
						DiscardHandle();
					}
					return RulerBoardTool.ClickResult.Finish;

				case ClickResponse.Finish:
					_values.Finish(CurrentIndicatorIndex);
					return RulerBoardTool.ClickResult.Finish;
			}
			throw new Exception("Invalid ClickResponse value");
		}

		private void OnDestroy()
		{
			Dispose();
		}

		public void OnDistanceUnitsChanged()
		{
			int index = _values.ActiveHandle(CurrentIndicatorIndex);
			SetHandle(index, _values.Get(CurrentIndicatorIndex, index));
		}

		public override void OnPhotonInstantiate(PhotonMessageInfo info)
		{
			_photonView = base.photonView;
			_remoteSync = !_photonView.isMine;
			if (_remoteSync)
			{
				Init(0);
			}
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			_tmpBuffer.Clear();
			if (stream.isWriting)
			{
				byte num = Convert.ToByte(_values.ActiveHandle(CurrentIndicatorIndex));
				byte thing = (byte)((_gmOnlyMode ? 0x40 : 0) | num);
				NativeHelpers.SerializeUnmanagedAndAppend<sbyte>(_tmpBuffer, Convert.ToSByte(CurrentIndicatorIndex));
				NativeHelpers.SerializeUnmanagedAndAppend(_tmpBuffer, thing);
				float3 num3 = _values.Get(CurrentIndicatorIndex, 0);
				NativeHelpers.SerializeUnmanagedAndAppend<float3>(_tmpBuffer, num3);
				int num4 = num + 1;
				for (int i = 1; i < num4; i++)
				{
					half3 half = (half3)(_values.Get(CurrentIndicatorIndex, i) - num3);
					NativeHelpers.SerializeUnmanagedAndAppend<half3>(_tmpBuffer, half);
				}
				stream.SendNext(_tmpBuffer.ToArray());
			}
			else
			{
				sbyte num7;
				byte num8;
				float3 num11;
				byte[] src = (byte[])stream.ReceiveNext();
				_tmpBuffer.AddRange<byte>(src);
				int byteOffset = 0;
				NativeHelpers.DeserializeUnmanagedAndAdvance<sbyte>((NativeArray<byte>)_tmpBuffer, ref byteOffset, out num7);
				NativeHelpers.DeserializeUnmanagedAndAdvance<byte>((NativeArray<byte>)_tmpBuffer, ref byteOffset, out num8);
				bool gmOnly = (num8 & 0x40) != 0;
				SetGmOnly(gmOnly);
				int num9 = num8 & -65;
				int num10 = _values.ActiveHandle(num7);
				NativeHelpers.DeserializeUnmanagedAndAdvance<float3>((NativeArray<byte>)_tmpBuffer, ref byteOffset, out num11);
				float3 num12 = _targetValues[0] = num11;
				int num13 = num9 + 1;
				for (int i = 1; i < num13; i++)
				{
					half3 half2;
					NativeHelpers.DeserializeUnmanagedAndAdvance<half3>((NativeArray<byte>)_tmpBuffer, ref byteOffset, out half2);
					float3 num16 = num12 + half2;
					num12 = _targetValues[i] = num16;
					if (num10 < i)
					{
						_values.Set(CurrentIndicatorIndex, i, num12);
					}
				}
				if ((num7 != CurrentIndicatorIndex) || (num9 < num10))
				{
					SetNewIndicator(num7, true);
				}
				else
				{
					for (int j = num10 + 1; j <= num9; j++)
					{
						_indicator.InitHandle(j);
						_indicator.SetHandle(j, _values.Get(CurrentIndicatorIndex, j));
					}
				}
			}
		}

		public void SetGmOnly(bool gmOnly)
		{
			if (gmOnly != _gmOnlyMode)
			{
				_gmOnlyMode = gmOnly;
			}
		}

		private void SetHandle(int index, float3 value)
		{
			if ((_indicator != null) && (index <= _values.MaybeClampHandle(CurrentIndicatorIndex, index)))
			{
				_values.Set(CurrentIndicatorIndex, index, value);
				_indicator.SetHandle(index, value);
			}
		}

		public void SetMode(int index)
		{
			if ((index >= 0) && (index < RulerIndicators.Length))
			{
				SetNewIndicator(index, false);
			}
		}

		private void SetNewIndicator(int prototypeIndex, bool forceRecreate)
		{
			if ((CurrentIndicatorIndex != prototypeIndex) || forceRecreate)
			{
				if (_indicator != null)
				{
					Destroy(_indicator.gameObject);
				}
				if ((prototypeIndex < 0) || (prototypeIndex >= RulerIndicators.Length))
				{
					CurrentIndicatorIndex = -1;
				}
				else
				{
					CurrentIndicatorIndex = prototypeIndex;
					_indicator = Instantiate<RulerIndicator>(RulerIndicators[prototypeIndex]);
					_indicator.transform.SetParent(_transform);
					_values.SetMaxHandleCount(CurrentIndicatorIndex, _indicator.Init());
					_indicator.SetActive(true);
					int num = _values.ActiveHandle(CurrentIndicatorIndex);
					for (int i = 0; i <= num; i++)
					{
						_indicator.InitHandle(i);
						_indicator.SetHandle(i, _values.Get(CurrentIndicatorIndex, i));
					}
				}
			}
		}

		public static Ruler Spawn(int startingMode, bool networkSyncd = false)
		{
			Ruler component = (networkSyncd ? PhotonNetwork.Instantiate("Rulers/PhotonRuler", (Vector3)math.float3(0), Quaternion.identity, 0) : ((GameObject)Instantiate(Resources.Load("Rulers/Ruler")))).GetComponent<Ruler>();
			component.Init(startingMode);
			return component;
		}

		private void Update()
		{
			if (_remoteSync)
			{
				int num = _values.ActiveHandle(CurrentIndicatorIndex);
				for (int i = 0; i <= num; i++)
				{
					SetHandle(i, math.lerp(_values.Get(CurrentIndicatorIndex, i), _targetValues[i], (float)0.5f));
				}
			}
		}

		public void UpdateActiveHandle()
		{
			int index = _values.ActiveHandle(CurrentIndicatorIndex);
			SetHandle(index, _indicator.SampleForHandle(index));
		}

		public void UpdateGizmo(int gizmoIndex)
		{
			int index = _values.MaybeClampHandle(CurrentIndicatorIndex, _indicator.GizmoIndexToHandleIndex(gizmoIndex));
			SetHandle(index, _indicator.SampleForHandle(index));
		}

		// Properties
		public int CurrentIndicatorIndex { get; private set; }

		// Nested Types
		public enum ClickResponse
		{
			Cancel,
			DiscardClick,
			NextHandle,
			DiscardHandle,
			DiscardHandleAndFinish,
			Finish
		}

		public class Values
		{
			// Fields
			private float3[,] _perIndicatorValues;
			private int[] _perIndicatorHandleHead;
			private int[] _perIndicatorMaxHandleCounts;
			private int[] _perIndicatorCommittedHandle;

			// Methods
			public Values(int indicatorCount)
			{
				_perIndicatorValues = new float3[indicatorCount, 10];
				_perIndicatorHandleHead = new int[indicatorCount];
				_perIndicatorMaxHandleCounts = new int[indicatorCount];
				_perIndicatorCommittedHandle = new int[indicatorCount];
				for (int i = 0; i < indicatorCount; i++)
				{
					_perIndicatorCommittedHandle[i] = -1;
				}
			}

			public int ActiveHandle(int indicatorIndex) =>
					MaybeClampHandle(indicatorIndex, _perIndicatorHandleHead[indicatorIndex]);

			public void Discard()
			{
				for (int i = 0; i < _perIndicatorHandleHead.Length; i++)
				{
					_perIndicatorHandleHead[i] = _perIndicatorCommittedHandle[i];
				}
			}

			public void Finish(int indicatorIndex)
			{
				int y = _perIndicatorHandleHead[indicatorIndex];
				int x = math.min(1, y);
				for (int i = 0; i < _perIndicatorHandleHead.Length; i++)
				{
					if (i == indicatorIndex)
					{
						_perIndicatorCommittedHandle[indicatorIndex] = y;
					}
					else
					{
						int num4 = _perIndicatorCommittedHandle[i];
						if (x > num4)
						{
							for (int j = 0; j <= x; j++)
							{
								_perIndicatorValues[i, j] = _perIndicatorValues[indicatorIndex, j];
							}
						}
						_perIndicatorHandleHead[i] = math.max(x, num4);
					}
				}
			}

			public float3 Get(int indicatorIndex, int valueIndex) =>
					_perIndicatorValues[indicatorIndex, valueIndex];

			public int MaybeClampHandle(int indicatorIndex, int val)
			{
				int num = _perIndicatorMaxHandleCounts[indicatorIndex];
				return ((num == -1) ? val : math.clamp(val, 0, num - 1));
			}

			public void Next(int indicatorIndex)
			{
				//Predicate<int> match = <> c.<> 9__8_0;
				//if (<> c.<> 9__8_0 == null)
				//{
				//	Predicate<int> local1 = <> c.<> 9__8_0;
				//	match = <> c.<> 9__8_0 = x => x < 2;
				//}
				//if (!Array.TrueForAll<int>(_perIndicatorHandleHead, match))
				//{
				//	int num3 = _perIndicatorHandleHead[indicatorIndex];
				//	_perIndicatorCommittedHandle[indicatorIndex] = num3;
				//	_perIndicatorHandleHead[indicatorIndex] = num3 + 1;
				//}
				//else
				//{
				//	int num = _perIndicatorHandleHead[indicatorIndex];
				//	for (int i = 0; i < _perIndicatorHandleHead.Length; i++)
				//	{
				//		_perIndicatorHandleHead[i] = num + 1;
				//		_perIndicatorCommittedHandle[i] = num;
				//		_perIndicatorValues[i, num] = _perIndicatorValues[indicatorIndex, num];
				//	}
				//}
			}

			public void Set(int indicatorIndex, int valueIndex, float3 value)
			{
				_perIndicatorHandleHead[indicatorIndex] = math.max(valueIndex, _perIndicatorHandleHead[indicatorIndex]);
				_perIndicatorValues[indicatorIndex, valueIndex] = value;
			}

			public void SetMaxHandleCount(int indicatorIndex, int maxCount)
			{
				_perIndicatorMaxHandleCounts[indicatorIndex] = maxCount;
			}

		//	// Nested Types
		//	[Serializable, CompilerGenerated]
		//	private sealed class <>c
  //      {
  //          // Fields
  //          public static readonly Ruler.Values.<>c<>9 = new Ruler.Values.<>c();
		//	public static Predicate<int> <>9__8_0;

  //          // Methods
  //          internal bool <Next>b__8_0(int x) =>
		//						x < 2;
		}
	}
}
