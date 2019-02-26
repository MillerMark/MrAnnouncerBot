using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTD2XX_NET;
namespace UsbRelay8Driver
{
	/// <summary>
	/// SainSmart USB Relay 8 Device Class
	/// This class handles the read/writes to a particular SainSmart USB
	/// relay 8 device.  Because of the nature of the writes/reads to the
	/// device with 8 relays.  It may work with device with less than
	/// 8 relays, but it has not been tested.
	/// </summary>
	class UsbRelay8
	{
		#region Constants
		const int BaudRate = 921600;
		const byte BitMask = 0xff;          // 8-bit mask
		const byte BitMode = FTDI.FT_BIT_MODES.FT_BIT_MODE_SYNC_BITBANG;
		const int RelayCount = 8;           // Maximum number of relays supported by device
		#endregion // Constants

		#region Private Variables
		FTDI _device;
		FTDI.FT_STATUS _status;
		byte _values;

		// FTD2 driver requires byte arrays, only a byte is needed for device
		byte[] _writeBuffer = new byte[2];
		byte[] _readBuffer = new byte[2];

		#endregion // Private Variables

		#region Public Properties
		public byte Values
		{
			get { return _values; }
		}
		#endregion // Public Properties

		#region Constuctors
		/// <summary>
		/// Create a USB Relay Device and open it by the serial number
		/// </summary>
		/// <param name="serialNumber">Device serial number</param>
		public UsbRelay8(String serialNumber)
		{
			// Open the relay device by serial number
			//  The serial number is always uniques, so the safest
			_device = new FTDI();
			_status = _device.OpenBySerialNumber(serialNumber);
			if (_status != FTDI.FT_STATUS.FT_OK)
			{
				throw new UsbRelayDeviceNotFoundException();
			}

			// Set the baud rate
			_status = _device.SetBaudRate(BaudRate);
			if (_status != FTDI.FT_STATUS.FT_OK)
			{
				throw new UsbRelayConfigurationException();
			}

			// Set the bit mode
			_status = _device.SetBitMode(BitMask, BitMode);
			if (_status != FTDI.FT_STATUS.FT_OK)
			{
				throw new UsbRelayConfigurationException();
			}

			// Clear all the relays
			// Note: From the Data Sheet, when in Sync Mode you can
			//  only read the interface pins when writing.  So start
			//  start out with all the values cleared.
			_values = 0x00;
			SetRelays(_values);
		}

		#endregion  // Constructors

		#region Public Methods
		/// <summary>
		/// Get the relay values
		/// </summary>
		/// <returns></returns>
		public byte GetRelayValues()
		{
			return _values;
		}

		public bool GetRelayValue(int relay)
		{
			byte values = GetRelayValues();

			byte mask = (byte)(~(byte)(1 << (relay - 1)));
			if ((mask & values) > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Set an individual relay value.
		/// </summary>
		/// <param name="relay">relay index, zero based</param>
		/// <param name="value">relay value</param>
		public void SetRelay(int relay, bool value)
		{
			// Relay sanity check
			if (relay < 0 || relay > RelayCount - 1)
			{
				throw new UsbRelayInvalidRelayException();
			}

			// Get the current values
			byte newValue = GetRelayValues();

			// Please note, that this only works for devices with up to 8 relays 
			// note: C# doesn't have a power operator, have to use a Math a method call. 
			byte bitValue = (byte)Math.Pow(2, relay);
			if (true == value)
			{
				// Setting a relay value, or it in.
				newValue |= bitValue;
			}
			else
			{
				// Clearing a relay value, AND it out
				newValue &= (byte)~bitValue;
			}
			_values = newValue;

			SetRelays(_values);
		}

		/// <summary>
		/// Set all the relay values, each relay is a bit
		/// </summary>
		/// <param name="values">combined relay values</param>
		public void SetRelays(byte values)
		{
			// Set the write buffer with the values
			_values = values;
			_writeBuffer[0] = values;

			// Write the value
			uint bytesWritten = 0;
			_status = _device.Write(_writeBuffer, 1, ref bytesWritten);
			if (_status != FTDI.FT_STATUS.FT_OK || bytesWritten != 1)
			{
				throw new UsbRelayWriteException();
			}
			Console.WriteLine(String.Format("SetRelays(0x{0:x2})", _values));
		}

		#endregion  // Public Methods
	}

	/// <summary>
	/// USB Relays Class.  
	/// This class holds a collection of relay devices and handles the setting
	/// of relays by using DeviceName:RelayNumber or assigned names for individual
	/// DeviceName:RelayNumber pairs.
	/// </summary>
	class UsbRelays
	{
		#region Constants
		#endregion      // Constants

		#region Private Variables
		FTDI _device;
		FTDI.FT_STATUS _status;
		FTDI.FT_DEVICE_INFO_NODE[] _deviceList;

		Dictionary<String, UsbRelay8> _relays;

		List<string> _serialNumbers;
		List<uint> _locationIds;
		List<uint> _ids;
		#endregion

		#region Properties

		/// <summary>
		/// Device Count
		/// </summary>
		public int DeviceCount
		{
			get { return GetDeviceCount(); }
		}

		/// <summary>
		/// Device List
		/// </summary>
		public FTDI.FT_DEVICE_INFO_NODE[] Devices
		{
			get
			{
				PopulateDeviceList();
				return _deviceList;
			}
		}

		/// <summary>
		/// Device serial numbers as a list of strings
		/// </summary>
		public List<String> DeviceSerialNumbers
		{
			get
			{
				PopulateDeviceList();
				return _serialNumbers;
			}
		}

		/// <summary>
		/// Device location IDs as list of uints
		/// </summary>
		public List<uint> DeviceLocationIDs
		{
			get
			{
				PopulateDeviceList();
				return _locationIds;
			}
		}

		/// <summary>
		/// Device IDs as a list of uints
		/// </summary>
		public List<uint> DeviceIDs
		{
			get
			{
				PopulateDeviceList();
				return _ids;
			}
		}

		#endregion //Properties

		#region Constructor
		public UsbRelays()
		{
			_device = new FTDI();
			_relays = new Dictionary<string, UsbRelay8>();
		}
		#endregion  // Constructor

		#region Public Methods

		public void OpenDevice(string serialNumber)
		{
			UsbRelay8 relay = new UsbRelay8(serialNumber);
			_relays.Add(serialNumber, relay);
		}

		public void SetRelay(string serialNumber, int relay, bool value)
		{
			_relays[serialNumber].SetRelay(relay, value);
		}

		public void SetRelays(string serialNumber, byte values)
		{
			_relays[serialNumber].SetRelays(values);
		}

		public bool GetRelay(string serialNumber, int relay)
		{
			return _relays[serialNumber].GetRelayValue(relay);
		}

		public byte GetRelays(string serialNumber)
		{
			return _relays[serialNumber].GetRelayValues();
		}

		#endregion  // Public Methods

		#region Private Methods
		/// <summary>
		/// Return the device count
		/// </summary>
		/// <returns>Number of devices</returns>
		private int GetDeviceCount()
		{
			uint count = 0;
			_status = _device.GetNumberOfDevices(ref count);
			if (_status != FTDI.FT_STATUS.FT_OK)
			{
				throw new UsbRelayStatusException();
			}

			return (int)count;
		}

		private void PopulateDeviceList()
		{
			// Have to create known size array to get the device list
			int count = GetDeviceCount();
			_deviceList = new FTDI.FT_DEVICE_INFO_NODE[count];
			_status = _device.GetDeviceList(_deviceList);
			if (_status != FTDI.FT_STATUS.FT_OK)
			{
				throw new UsbRelayStatusException();
			}

			// Create new lists to populate
			_serialNumbers = new List<string>();
			_locationIds = new List<uint>();
			_ids = new List<uint>();

			// Populate each list
			foreach (FTDI.FT_DEVICE_INFO_NODE device in _deviceList)
			{
				_serialNumbers.Add(device.SerialNumber);
				Console.WriteLine("Serial Number: " + device.SerialNumber);

				_locationIds.Add(device.LocId);
				Console.WriteLine("Location ID: " + device.LocId.ToString());

				_ids.Add(device.ID);
				Console.WriteLine("ID: " + device.ID.ToString());
			}
		}

		#endregion      // Private Methods

	}
}