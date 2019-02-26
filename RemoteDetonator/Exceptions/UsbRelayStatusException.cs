using System;
using System.Linq;



namespace UsbRelay8Driver

{
	class UsbRelayStatusException : Exception

	{

		public override string Message

		{

			get { return "SainSmart USB status not OK error."; }

		}

	}
}