using System;
using System.Linq;
using SheetsPersist;

namespace DHDM
{
	public static class BluetoothLights
	{
		static string LeftID => "E286CF5DEEBA";
		static string RightID => "F008BE401CCF";

		public static string Left_ID => LeftID;
		public static string Right_ID => RightID;

		public static BluetoothLight Left { get; set; } = new BluetoothLight(LeftID);
		public static BluetoothLight Right { get; set; } = new BluetoothLight(RightID);
	}
}
