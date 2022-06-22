using SharpDX.DirectInput;
using System.Diagnostics;
using System;
using System.Threading;

namespace DHDM.FlyMark;

internal static class JoystickListener
{
	static bool listening;
	static DirectInput directInput;
	static Joystick joystick;
	static double lastTiltSent;
	static double lastThrustSent;

	public static Joystick Joystick {
		get
		{
			if (joystick != null)
				return joystick;

			if (directInput == null)
				directInput = new DirectInput();

			Guid joystickGuid = GetJoystickGuid();

			if (joystickGuid == Guid.Empty)
			{
				Debug.WriteLine("No joystick/Gamepad found.");
				return null;
			}

			joystick = InstantiateJoystick(joystickGuid);

			return joystick;
		}
	}

	private static Joystick InstantiateJoystick(Guid joystickGuid)
	{
		var joystick = new Joystick(directInput, joystickGuid);
		joystick.Properties.BufferSize = 128;
		joystick.Acquire();
		return joystick;
	}

	private static Guid GetJoystickGuid()
	{
		var joystickGuid = Guid.Empty;

		foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad,
					DeviceEnumerationFlags.AllDevices))
			joystickGuid = deviceInstance.InstanceGuid;

		// If Gamepad not found, look for a Joystick
		if (joystickGuid == Guid.Empty)
			foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick,
					DeviceEnumerationFlags.AllDevices))
				joystickGuid = deviceInstance.InstanceGuid;
		return joystickGuid;
	}

	public static void StartListening()
	{
		if (listening)
			return;
		if (Joystick == null)
		{
			Debug.WriteLine($"Joystick not found!");
			return;
		}
		listening = true;
		new Thread(() =>
		{
			Thread.CurrentThread.IsBackground = true;

			while (listening)
			{
				GetLatestJoystickPosition();
				Thread.Sleep(300);
			}

		}).Start();
	}

	public static void StopListening()
	{
		listening = false;
	}

	static double Map16Bit(double value, double min, double max)
	{
		return min + (value / 65536) * (max - min);
	}

	static double SnapTo(double value, double snapValue)
	{
		return Math.Round(value / snapValue) * snapValue;
	}

	static void Invalidate()
	{
		joystick = null;
	}

	static void GetLatestJoystickPosition()
	{
		Joystick joystick = Joystick;
		if (joystick == null)
			return;
		joystick.Poll();
		JoystickUpdate[] data;
		try
		{
			
			data = joystick.GetBufferedData();
		}
		catch 
		{
			Invalidate();
			return;
		}
		double newTiltDegrees = double.MinValue;
		double newThrust = double.MinValue;

		foreach (var state in data)
		{
			if (state.Offset == JoystickOffset.RotationX)
				newTiltDegrees = SnapTo(Math.Round(Map16Bit(state.Value, -90, 90)), 5);

			if (state.Offset == JoystickOffset.RotationY)
				newThrust = Math.Round(Map16Bit(state.Value, 9, 1));

			Debug.WriteLine(state);
		}

		if (newTiltDegrees != double.MinValue && newTiltDegrees != lastTiltSent)
		{
			lastTiltSent = newTiltDegrees;
			BotCore.Twitch.DroneCommandsChat($"!tilt {newTiltDegrees}");
		}
		if (newThrust != double.MinValue && newThrust != lastThrustSent)
		{
			lastThrustSent = newThrust;
			BotCore.Twitch.DroneCommandsChat($"!thrust {newThrust}");
		}
	}
}
