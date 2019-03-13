using Microsoft.AspNetCore.SignalR.Client;
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

		static void FireRelay(int relayNum)
		{
			try
			{
				const int msDelay = 7000;
				Console.BackgroundColor = ConsoleColor.DarkRed;
				Console.WriteLine("Firing Relay #" + relayNum);
				SetTargetRelay(relayNum, true);
				Thread.Sleep(msDelay);
			}
			finally
			{
				Console.BackgroundColor = ConsoleColor.Black;
				Console.WriteLine("Turning Off Relay #" + relayNum);
				SetTargetRelay(relayNum, false);
			}
		}

		static int nextRelayNumToFire = 0;

		static bool IsSuperUser(string userID)
		{
			return userID == "237584851";
		}

		static void Arm(string userID)
		{
			if (!IsSuperUser(userID))
				return;
			armed = true;
			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.DarkRed;
			Console.WriteLine("           ");
			Console.WriteLine(" A R M E D ");
			Console.WriteLine("           ");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.BackgroundColor = ConsoleColor.Black;
			nextRelayNumToFire = 1;
		}

		static void Disarm(string userID)
		{
			if (!IsSuperUser(userID))
				return;
			armed = false;

			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.DarkBlue;
			Console.WriteLine("                 ");
			Console.WriteLine(" D I S A R M E D ");
			Console.WriteLine("                 ");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.BackgroundColor = ConsoleColor.Black;
		}

		static void Fire(string userID)
		{
			if (!armed)
				return;
			FireRelay(nextRelayNumToFire);
			nextRelayNumToFire++;
		}

		static void ConnectToSignalR()
		{
			hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:44303/MrAnnouncerBotHub").Build();
			if (hubConnection != null)
			{
				//hubConnection.Closed += HubConnection_Closed;
				hubConnection.On<string>("Arm", Arm);
				hubConnection.On<string>("Disarm", Disarm);
				hubConnection.On<string>("Fire", Fire);
				// TODO: Check out benefits of stopping gracefully with a cancellation token.
				hubConnection.StartAsync();
			}
		}
		static void Main(string[] args)
		{
			ConnectToSignalR();
			ConnectToRelays();

			//Console.WriteLine("Press Enter to Blast off #1.");
			//Console.ReadLine();

			//FireRelay(1);

			//Console.WriteLine("Press Enter to exit.");
			Console.ReadLine();
		}

		static UsbRelays _relays;
		static List<Relay> _relayControls;
		static HubConnection hubConnection;
		static bool armed;

		private static void ConnectToRelays()
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
