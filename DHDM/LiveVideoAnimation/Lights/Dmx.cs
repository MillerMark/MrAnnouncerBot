using System;
using System.Linq;
using OpenDMX.NET;

namespace DHDM
{
	public static class Dmx
	{
		static DmxController dmxController = new OpenDMX.NET.DmxController();

		public static DmxController Controller { get => dmxController; }

		static Dmx()
		{
			dmxController.Open(0);
		}

		public static void ShutDown()
		{
			dmxController.Dispose();
		}

	}
}
