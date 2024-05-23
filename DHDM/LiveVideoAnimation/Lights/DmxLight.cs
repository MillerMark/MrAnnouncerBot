using Imaging;
using OpenDMX.NET;
using System;
using System.Linq;
using System.Windows.Media;

namespace DHDM
{
	public class DmxLight
	{
        public static bool Enabled { get; set; }
        static DmxLight left;
		public static DmxLight Left
		{
			get
			{
				if (left == null)
					left = new DmxLight(10);
				return left;
			}
		}
		static DmxLight right;
		public static DmxLight Right
		{
			get
			{
				if (right == null)
					right = new DmxLight(30);
				return right;
			}
		}
		static DmxLight center;
		public static DmxLight Center
		{
			get
			{
				if (center == null)
					center = new DmxLight(20);
				return center;
			}
		}
		public DmxLight(int channelStart)
		{
			ChannelStart = channelStart;
		}

		public void SetColor(string htmlColorStr)
		{
			HueSatLight hueSatLight = new HueSatLight(htmlColorStr);
			SetColor(hueSatLight);
		}

		private void SetColor(HueSatLight hueSatLight)
		{
            if (!Enabled)
                return;
			Color asRGB = hueSatLight.AsRGB;
			Dmx.Controller.SetChannel(ChannelStart, asRGB.R);
			Dmx.Controller.SetChannel(ChannelStart + 1, asRGB.G);
			Dmx.Controller.SetChannel(ChannelStart + 2, asRGB.B);
		}

		public void SetColor(LightSequenceData lightData)
		{
			SetColor(lightData.Hue, lightData.Saturation, lightData.Lightness);
		}

		public void SetColor(double hue, double saturation, double lightness)
		{
			SetColor(new HueSatLight(hue / 360.0, saturation / 100.0, lightness / 100.0));
		}

        public static DmxLight? GetFrom(string id)
        {
            if (id == BluetoothLights.Left_ID)
                return Left;
            else if (id == BluetoothLights.Right_ID)
                return Right;
            else if (id == BluetoothLights.Center_ID)
                return Center;
            return null;
        }

        public int ChannelStart { get; set; }
	}
}
