using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;

namespace DHDM
{
	public static class HubtasticBaseStation
	{
		static readonly object hubConnectionLock = new object();
		static HubConnection hubConnection;

		public static HubConnection HubConnection
		{
			get
			{
				if (hubConnection == null)
				{
					lock(hubConnectionLock)
					{
						if (hubConnection == null)  // thread safety is important, kids.
						{
							hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:44303/MrAnnouncerBotHub").Build();
							if (hubConnection != null)
							{
								//hubConnection.Closed += HubConnection_Closed;
								// TODO: Check out benefits of stopping gracefully with a cancellation token.
								hubConnection.StartAsync();
							}
						}
					}
				}
				return hubConnection;
			}
		}

		public static void PlayerDataChanged(int playerID, ScrollPage pageID, string playerData)
		{
			HubConnection.InvokeAsync("PlayerDataChanged", playerID, (int)pageID, playerData);
		}

		public static void FocusItem(int playerID, ScrollPage pageID, string itemID)
		{
			HubConnection.InvokeAsync("FocusItem", playerID, (int)pageID, itemID);
		}

		public static void UnfocusItem(int playerID, ScrollPage pageID, string itemID)
		{
			HubConnection.InvokeAsync("UnfocusItem", playerID, (int)pageID, itemID);
		}

		public static void TriggerEffect(string effectData)
		{
			HubConnection.InvokeAsync("TriggerEffect", effectData);
		}
	}
}
