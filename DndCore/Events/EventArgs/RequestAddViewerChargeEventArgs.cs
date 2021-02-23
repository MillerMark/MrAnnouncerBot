using System;

namespace DndCore
{
	public class RequestAddViewerChargeEventArgs : EventArgs
	{
		public string UserName { get; set; }
		public string ChargeName { get; set; }
		public int ChargeCount { get; set; }
		public RequestAddViewerChargeEventArgs()
		{

		}
		public RequestAddViewerChargeEventArgs(string userName, string chargeName, int chargeCount)
		{
			UserName = userName;
			ChargeName = chargeName;
			ChargeCount = chargeCount;
		}
	}
}
