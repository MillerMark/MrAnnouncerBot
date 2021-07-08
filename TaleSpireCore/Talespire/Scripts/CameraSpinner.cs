using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore.Scripts
{
	/// <summary>
	/// Class to move the TaleSpire camera. 
	/// Key     Behavior
	///  -      Spins the camera counter-clockwise 
	///  =      Spins the camera clockwise 
	///  0      Slows the spinning camera to a stop
	///  9      Moves the camera up
	///  y      Moves the camera down
	/// </summary>
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
		float heightAdjust;
		bool StoppingSoon;

		void FixedUpdate()
		{
			heightAdjust = 0f;
			float spinAdjust;
			if (Mathf.Abs(SpinRate) < 1.5)
				spinAdjust = 0.1f;
			else if (Mathf.Abs(SpinRate) < 1.5)
				spinAdjust = 0.25f;
			else if (Mathf.Abs(SpinRate) < 5)
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

			if (StoppingSoon)
			{
				SpinRate *= 0.95f;
				if (Mathf.Abs(SpinRate) <= 0.5f)
				{
					SpinRate = 0;
					StoppingSoon = false;
				}
			}

			if (Input.GetKey(KeyCode.Equals))
			{
				StoppingSoon = false;
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
				StoppingSoon = false;
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
			else if (Input.GetKey(KeyCode.Alpha0))
			{
				StoppingSoon = true;
			}
			else if (Input.GetKey(KeyCode.Alpha6))
			{
				heightAdjust = 0.8f;
			}
			else if (Input.GetKey(KeyCode.G))
			{
				heightAdjust = -0.8f;
			}

			if (SpinRate == 0 && heightAdjust == 0)
				return;

			float secondsSinceLastUpdate = Time.time - lastUpdateTime;
			
			if (SpinRate != 0)
			{
				float rotateY = SpinRate * secondsSinceLastUpdate;
				transform.Rotate(Vector3.up, rotateY);
			}

			if (heightAdjust != 0)
			{
				float deltaHeight = heightAdjust * secondsSinceLastUpdate;
				float newHeight = CameraController.CameraHeight + deltaHeight;
				if (newHeight > 60f)
				{
					newHeight = 60f;
					heightAdjust = 0;
				}
				else if (newHeight < 0f)
				{
					newHeight = 0f;
					heightAdjust = 0;
				}

				CameraController.MoveToHeight(newHeight, false);
			}
			lastUpdateTime = Time.time;
		}

		private static bool ShiftKeyDown()
		{
			return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		}
	}
}