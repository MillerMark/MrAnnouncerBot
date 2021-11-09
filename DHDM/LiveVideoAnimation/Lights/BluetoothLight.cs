using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using InTheHand.Bluetooth;

namespace DHDM
{
	public class BluetoothLight
	{
		private string LightID;
		BluetoothDevice device;

		public BluetoothLight()
		{
		}

		public BluetoothLight(string lightID)
		{
			LightID = lightID;
		}

		public async Task SetAsync(int hue, int saturation, int lightness)
		{
			await Send(GetHslValue(hue, saturation, lightness));
		}

		async Task GetAllDevices()
		{
			RequestDeviceOptions ro = new RequestDeviceOptions();
			ro.AcceptAllDevices = true;
			// Seems to help get the devices when we start and cannot find them.
			IReadOnlyCollection<BluetoothDevice> scannedDevices = await Bluetooth.ScanForDevicesAsync(ro);
		}

		async Task Send(byte[] byteArray)
		{
			await GetDevice();
			if (device == null)
				return;

			const string STR_LightControlServiceId = "69400001-b5a3-f393-e0a9-e50e24dcca99";
			BluetoothUuid bluetoothUuid = BluetoothUuid.FromGuid(new Guid(STR_LightControlServiceId));
			GattService controlService = await device.Gatt.GetPrimaryServiceAsync(BluetoothUuid.FromGuid(new Guid(STR_LightControlServiceId)));
			if (controlService != null)
			{
				const string STR_CharacteristicID = "69400002-b5a3-f393-e0a9-e50e24dcca99";
				GattCharacteristic gattCharacteristic = await controlService.GetCharacteristicAsync(BluetoothUuid.FromGuid(new Guid(STR_CharacteristicID)));
				await gattCharacteristic.WriteValueWithoutResponseAsync(byteArray);
			}
		}

		private async Task GetDevice()
		{
			if (device != null)
				return;

			device = await BluetoothDevice.FromIdAsync(LightID);
			if (device != null)
				return;

			await GetAllDevices();
			device = await BluetoothDevice.FromIdAsync(LightID);
		}

		/// <summary>
		/// hue is expected to be between 0 and 360. 
		/// saturation between 0 and 100. 
		/// lightness between 0 and 100.
		/// </summary>
		byte[] GetHslValue(int hue, int saturation, int lightness)
		{
			const int byteCount = 4;

			byte[] byteArray = new byte[byteCount + 4];

			byte cmd_prefix_tag = 0x78;  // 120
			byte cmd_set_hsi_light_tag = 0x86;  // Set HSI Light Mode.
			byteArray[0] = cmd_prefix_tag;
			byteArray[1] = cmd_set_hsi_light_tag;
			byteArray[2] = byteCount;

			// 4 elements:
			byteArray[3] = (byte)(hue & 0xFF);
			byteArray[4] = (byte)((hue & 0xFF00) >> 8);
			byteArray[5] = (byte)saturation; // saturation 0x00 ~ 0x64 (100)
			byteArray[6] = (byte)lightness; // lightness

			int checkSum = 0;
			for (int i = 0; i <= 6; i++)
				checkSum += byteArray[i];

			byteArray[7] = (byte)(checkSum & 0xff);
			return byteArray;
		}

		public async Task OnAsync()
		{
			byte[] cmd_power_on = new byte[5] { 0x78, 0x81, 0x01, 0x01, 0xFB };
			await Send(cmd_power_on);
		}

		public async Task OffAsync()
		{
			byte[] cmd_power_off = new byte[5] { 0x78, 0x81, 0x01, 0x02, 0xFC };
			await Send(cmd_power_off);
		}
	}
}
