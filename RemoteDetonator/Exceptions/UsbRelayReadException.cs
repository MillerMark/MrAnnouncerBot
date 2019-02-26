using System;
using System.Linq;





// TODO: List
// TODO: 1) Add the ability to add a relay device by serial number
// TODO: 2) Add the ability to set a relay by serial number and relay number


namespace UsbRelay8Driver

{
	class UsbRelayReadException : Exception

	{

		public override string Message

		{

			get { return "SainSmart USB relay read error."; }

		}

	}
}