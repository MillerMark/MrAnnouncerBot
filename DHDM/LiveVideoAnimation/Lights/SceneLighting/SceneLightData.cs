using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;
using Imaging;

namespace DHDM
{
	[SheetName("Live Video Animation")]
	[TabName("Lights")]
	public class SceneLightData
	{
		public string LeftLight { get; set; }
		public string RightLight { get; set; }
		public string SceneName { get; set; }

		public async void SetLightsNow()
		{
			HueSatLight leftColor = new HueSatLight(LeftLight);
			await BluetoothLights.Left.SetAsync((int)(leftColor.Hue * 360), (int)(leftColor.Saturation * 100), (int)(100 * leftColor.Lightness));

			HueSatLight rightColor = new HueSatLight(RightLight);
			await BluetoothLights.Right.SetAsync((int)(rightColor.Hue * 360), (int)(rightColor.Saturation * 100), (int)(100 * rightColor.Lightness));
		}

		public SceneLightData()
		{

		}
	}
}
