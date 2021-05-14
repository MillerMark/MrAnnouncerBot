using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Flashlight
		{
			public static bool IsOff()
			{
				FlashLight flashLight = Get();
				return flashLight == null;
			}

			public static bool IsOn()
			{
				FlashLight flashLight = Get();
				return flashLight != null;
			}

			public static void TurnOn()
			{
				if (IsOff())
					Toggle();
			}
			public static void TurnOff()
			{
				if (IsOn())
					Toggle();
			}

			private static void Toggle()
			{
				ReflectionHelper.CallNonPublicMethod(typeof(LocalClient), "ToggleFlashLight");
			}

			public static string GetPositionStr()
			{
				FieldInfo flashlightFieldInfo = GetFieldInfo();
				if (flashlightFieldInfo == null)
					return "Error: _flashLight not found!";

				FlashLight flashLight = Get(flashlightFieldInfo);
				if (flashLight == null)
					return "off";

				if (flashLight.gameObject == null)
					return "Error: flashLight.gameObject == null!";

				Transform transform = flashLight.gameObject.GetComponent<Transform>();
				if (transform == null)
					return "Error: Flashlight found but no transform!";

				return $"({transform.position.x:f}, {transform.position.y:f}, {transform.position.z:f})";
			}

			/// <summary>
			/// Returns the position of the flashlight on the client's machine. 
			/// Returns Vector3.negativeInfinity if the flashlight is off or not found.
			/// </summary>
			/// <returns></returns>
			public static Vector3 GetPosition()
			{
				FieldInfo flashlightFieldInfo = GetFieldInfo();
				if (flashlightFieldInfo == null)
					return Vector3.negativeInfinity;

				FlashLight flashLight = Get(flashlightFieldInfo);
				if (flashLight == null)
					return Vector3.negativeInfinity;

				if (flashLight.gameObject == null)
					return Vector3.negativeInfinity;

				Transform transform = flashLight.gameObject.GetComponent<Transform>();
				if (transform == null)
					return Vector3.negativeInfinity;

				return new Vector3(transform.position.x, transform.position.y, transform.position.z);
			}

			public static FlashLight Get()
			{
				FieldInfo flashlightFieldInfo = GetFieldInfo();
				if (flashlightFieldInfo == null)
					return null;
				return Get(flashlightFieldInfo);
			}

			private static FlashLight Get(FieldInfo flashlightFieldInfo)
			{
				if (flashlightFieldInfo.GetValue(null) is FlashLight flashLight)
				{
					return flashLight;
				}
				return null;
			}

			private static FieldInfo GetFieldInfo()
			{
				return typeof(LocalClient).GetField("_flashLight", BindingFlags.NonPublic | BindingFlags.Static);
			}
		}
	}
}