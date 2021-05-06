using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using UnityEngine;

namespace TaleSpireCore
{
	public static class TaleSpireHelper
	{
		public static void SwitchToSpectatorMode()
		{
			LocalClient.SetLocalClientMode(ClientMode.Spectator);
		}

		public static void SwitchToPlayerMode()
		{
			LocalClient.SetLocalClientMode(ClientMode.Player);
		}

		public static bool FlashLightIsOff()
		{
			FlashLight flashLight = GetFlashlight();
			return flashLight == null;
		}

		public static bool FlashLightIsOn()
		{
			FlashLight flashLight = GetFlashlight();
			return flashLight != null;
		}

		public static void TurnFlashlightOn()
		{
			if (FlashLightIsOff())
				ToggleFlashLight();
		}
		public static void TurnFlashlightOff()
		{
			if (FlashLightIsOn())
				ToggleFlashLight();
		}

		private static void ToggleFlashLight()
		{
			ReflectionHelper.CallNonPublicMethod(typeof(LocalClient), "ToggleFlashLight");
		}

		public static string GetFlashlightPositionStr()
		{
			FieldInfo flashlightFieldInfo = GetFlashlightFieldInfo();
			if (flashlightFieldInfo == null)
				return "Error: _flashLight not found!";

			FlashLight flashLight = GetFlashlight(flashlightFieldInfo);
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
		public static Vector3 GetFlashlightPosition()
		{
			FieldInfo flashlightFieldInfo = GetFlashlightFieldInfo();
			if (flashlightFieldInfo == null)
				return Vector3.negativeInfinity;

			FlashLight flashLight = GetFlashlight(flashlightFieldInfo);
			if (flashLight == null)
				return Vector3.negativeInfinity;

			if (flashLight.gameObject == null)
				return Vector3.negativeInfinity;

			Transform transform = flashLight.gameObject.GetComponent<Transform>();
			if (transform == null)
				return Vector3.negativeInfinity;

			return new Vector3(transform.position.x, transform.position.y, transform.position.z);
		}

		public static FlashLight GetFlashlight()
		{
			FieldInfo flashlightFieldInfo = GetFlashlightFieldInfo();
			if (flashlightFieldInfo == null)
				return null;
			return GetFlashlight(flashlightFieldInfo);
		}

		private static FlashLight GetFlashlight(FieldInfo flashlightFieldInfo)
		{
			if (flashlightFieldInfo.GetValue(null) is FlashLight flashLight)
			{
				return flashLight;
			}
			return null;
		}

		private static FieldInfo GetFlashlightFieldInfo()
		{
			return typeof(LocalClient).GetField("_flashLight", BindingFlags.NonPublic | BindingFlags.Static);
		}
	}
}
