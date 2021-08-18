using Unity.Mathematics;
using UnityEngine;

namespace TaleSpireExploreDecompiles
{
	public abstract class RulerIndicator : MonoBehaviour
	{
		// Fields
		private Ruler _ruler;
		private int _maxHandleInitd = -1;

		// Methods
		protected RulerIndicator()
		{
		}

		public int BaseInit(Ruler ruler)
		{
			_ruler = ruler;
			return Init();
		}

		public virtual int GizmoIndexToHandleIndex(int gizmoIndex) =>
				gizmoIndex;

		public abstract int HandleGizmoHover(Ray cameraRay);
		public abstract int Init();
		public void InitHandle(int index)
		{
			StartingSetHandle(index);
		}

		public abstract Ruler.ClickResponse OnClick(int index, int buttonId);
		public abstract float3 SampleForHandle(int index);
		public void SetActive(bool val)
		{
			base.gameObject.SetActive(val);
		}

		public abstract void SetHandle(int index, float3 pos);
		protected abstract void StartingSetHandle(int index);

		// Properties
		public virtual bool SupportsReturningToEdit => false;
	}
}