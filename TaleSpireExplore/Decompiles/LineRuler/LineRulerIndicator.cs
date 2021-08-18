using System.Collections.Generic;
using System.Globalization;
using Unity.Mathematics;
using UnityEngine;

namespace TaleSpireExploreDecompiles
{
	public class LineRulerIndicator : RulerIndicator
	{
		// Fields
		[SerializeField]
		private LineIndicator _lineIndicator;
		[SerializeField]
		private Sprite _lineIcon;
		[SerializeField]
		private Material _indicatorMaterial;
		private readonly List<Transform> _handles = new List<Transform>();
		private Transform _transform;
		private UIInGameText.HiredText _hiredText;
		private static float3 _localOffset = new float3(0f, 0.03f, 0f);

		// Methods
		public override int HandleGizmoHover(Ray cameraRay)
		{
			int num = -1;
			float num2 = 0.25f;
			int count = _handles.Count;
			for (int i = 0; i < count; i++)
			{
				float num5 = Mathfx.DistanceToRay(cameraRay, _handles[i].position);
				if (num5 < num2)
				{
					num = i;
					num2 = num5;
				}
			}
			return num;
		}

		public override int Init()
		{
			_transform = base.transform;
			return -1;
		}

		public override Ruler.ClickResponse OnClick(int index, int buttonId)
		{
			if (buttonId == 0)
			{
				return Ruler.ClickResponse.NextHandle;
			}
			if (buttonId != 1)
			{
				return Ruler.ClickResponse.DiscardClick;
			}
			if (index < 2)
			{
				return Ruler.ClickResponse.Cancel;
			}
			if (index != (_handles.Count - 1))
			{
				return Ruler.ClickResponse.Finish;
			}
			_handles.RemoveAt(_handles.Count - 1);
			_lineIndicator.SetTransformPoints(_handles);
			return Ruler.ClickResponse.DiscardHandleAndFinish;
		}

		private void OnDisable()
		{
			if (_hiredText == null)
			{
				UIInGameText.HiredText local1 = _hiredText;
			}
			else
			{
				_hiredText.Retire();
			}
			_lineIndicator.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			_lineIndicator.gameObject.SetActive(true);
		}

		public override float3 SampleForHandle(int index) =>
				RulerHelpers.GetPointerPos();

		public override void SetHandle(int index, float3 pos)
		{
			_handles[index].position = (Vector3)(pos + _localOffset);
			if ((_hiredText != null) && (_handles.Count > 1))
			{
				float num = 0f;
				int num3 = 1;
				while (true)
				{
					if (num3 >= _handles.Count)
					{
						_hiredText.SetWorldPosition(_handles[_handles.Count - 1].position);
						_hiredText.Text = (num * CampaignSessionManager.DistanceUnits[0].NumberPerTile).ToString(".0 " + CampaignSessionManager.DistanceUnits[0].Name, CultureInfo.CurrentCulture);
						break;
					}
					num += Vector3.Distance(_handles[num3 - 1].localPosition, _handles[num3].localPosition);
					num3++;
				}
			}
		}

		protected override void StartingSetHandle(int index)
		{
			GameObject obj2 = new GameObject("handle");
			Transform item = obj2.transform;
			item.SetParent(_transform);
			if (index != 0)
			{
				if (index != 1)
				{
					item.localPosition = _handles[index - 1].localPosition;
				}
				else
				{
					item.localPosition = _handles[index - 1].localPosition;
					if (_hiredText == null)
					{
						_hiredText = SingletonBehaviour<GUIManager>.Instance.InGameText.HireText("", float3.zero);
						_hiredText.SetSprite(_lineIcon);
					}
				}
			}
			obj2.SetActive(true);
			_handles.Add(item);
			GameObject obj1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			Transform transform = obj1.transform;
			transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			transform.SetParent(item, false);
			transform.GetComponent<MeshRenderer>().sharedMaterial = _indicatorMaterial;
			obj1.SetActive(true);
			_lineIndicator.SetTransformPoints(_handles);
		}

		// Properties
		public override bool SupportsReturningToEdit =>
				true;
	}
}