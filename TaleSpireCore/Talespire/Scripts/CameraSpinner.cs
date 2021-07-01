using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore.Scripts
{
	public class CameraSpinner : MonoBehaviour
	{
		public float SpinRate = 0;
		public float TargetSpinRate = float.MinValue;

		// Start is called before the first frame update
		void Start()
		{
			lastUpdateTime = Time.time;
		}

		float lastUpdateTime;

		void FixedUpdate()
		{
			float spinAdjust;
			if (Mathf.Abs(SpinRate) < 1.5)
				spinAdjust = 0.25f;
			else if (Mathf.Abs(SpinRate) < 3)
				spinAdjust = 0.5f;
			else 
				spinAdjust = 1;

			if (TargetSpinRate != float.MinValue)
				if (TargetSpinRate != SpinRate)
				{
					float delta = TargetSpinRate - SpinRate;
					float absDelta = Math.Abs(delta);
					if (absDelta > 1)
						SpinRate += spinAdjust * Math.Sign(delta);
					else if (absDelta > 0)
					{
						SpinRate = TargetSpinRate;
						TargetSpinRate = float.MinValue;
					}
				}
				else
					TargetSpinRate = float.MinValue;

			if (Input.GetKey(KeyCode.Equals))
			{
				TargetSpinRate = float.MinValue;
				if (ShiftKeyDown())
				{
					// We are stopping at 0.
					if (SpinRate <= 0)
						if (SpinRate >= -1)
							SpinRate = 0;
						else
							SpinRate += spinAdjust;
					else
						SpinRate += spinAdjust;
				}
				else
					SpinRate += spinAdjust;
			}
			else if (Input.GetKey(KeyCode.Minus))
			{
				TargetSpinRate = float.MinValue;
				if (ShiftKeyDown())
				{
					// We are stopping at 0.
					if (SpinRate >= 0)
						if (SpinRate <= 1)
							SpinRate = 0;
						else
							SpinRate -= spinAdjust;
					else
						SpinRate -= spinAdjust;
				}
				else
					SpinRate -= spinAdjust;
			}

			if (SpinRate == 0)
				return;
			float secondsSinceLastUpdate = Time.time - lastUpdateTime;
			float newY = SpinRate * secondsSinceLastUpdate;
			transform.Rotate(Vector3.up, newY);
			lastUpdateTime = Time.time;
		}

		private static bool ShiftKeyDown()
		{
			return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		}
	}
}