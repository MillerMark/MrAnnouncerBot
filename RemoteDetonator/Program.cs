using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UsbRelay8Driver;
namespace RemoteDetonator
{
	class Program
	{
		private const string SerialNumber = "A601AYI4";
		static void Main(string[] args)
		{
			Connect();
			const int msDelay = 200;
			Console.WriteLine("Press Enter to set relay 1 to True for {0}ms.", msDelay);
			Console.ReadLine();
			SetTargetRelay(1, true);
			Thread.Sleep(msDelay);
			SetTargetRelay(1, false);
			Console.ReadLine();
		}

		static UsbRelays _relays;
		static List<Relay> _relayControls;

		private static void Connect()
		{
			_relays = new UsbRelays();

			// Populate the Device Count
			_relays.DeviceCount.ToString();

			// Open each device
			foreach (string device in _relays.DeviceSerialNumbers)
			{
				_relays.OpenDevice(device);
			}

			// Populate the relay check Boxes
			CreateRelayCheckBoxes();

			// Populate the USB Relay Devices Drop Down
			Console.WriteLine("Serial Numbers");
			foreach (string deviceSerialNumber in _relays.DeviceSerialNumbers)
			{
				Console.WriteLine("  " + deviceSerialNumber);
			}
			Console.WriteLine();
		}


		private static void CreateRelayCheckBoxes()
		{
			_relayControls = new List<Relay>();

			for (int i = 0; i < 8; i++)
			{
				// Cheat and use the tag to store the relay number
				//  so we don't have to keep track of the relay
				Relay relay = new Relay();
				relay.Name = "Relay" + i.ToString();
				relay.Text = "Relay " + i.ToString();
				relay.Index = i;
				_relayControls.Add(relay);
			}
		}


		private static void UpdateRelays(string sn)
		{
			if (sn == "")
			{
				return;
			}

			byte values = _relays.GetRelays(sn);
			//Console.WriteLine("SN: " + sn + "  Values: " + values.ToString());

			int relayNum = 0;
			foreach (Relay relay in _relayControls)
			{
				byte mask = (byte)(1 << relayNum);
				relayNum++;

				//Console.WriteLine("relayNum: " + relayNum.ToString() +
				//    String.Format("  values {0:x}", values) + 
				//    String.Format("  mask {0:x}", mask) );

				if ((mask & values) > 0)
				{
					relay.Latched = true;
				}
				else
				{
					relay.Latched = false;
				}
			}
		}


		private static void SetTargetRelay(int relayIndex, bool value)
		{

			_relays.SetRelay(SerialNumber, relayIndex, value);
		}

		private void cbDevices_SelectedIndexChanged(object sender, EventArgs e)
		{
			//Console.WriteLine("Relay item changed!");
			//UpdateRelays();
		}
	}
}
