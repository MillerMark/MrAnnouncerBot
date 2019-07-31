using DndUI;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;

namespace DHDM
{
	public static class HubtasticBaseStation
	{
		public delegate void DiceEventHandler(object sender, DiceEventArgs ea);
		static readonly object hubConnectionLock = new object();
		static HubConnection hubConnection;
		public static event DiceEventHandler DiceStoppedRolling;
		public static void OnDiceStoppedRolling(object sender, DiceEventArgs ea)
		{
			DiceStoppedRolling?.Invoke(sender, ea);
		}
		static DiceEventArgs diceEventArgs;
		static void DiceHaveStoppedRolling(string diceData)
		{
			if (diceEventArgs == null)
				diceEventArgs = new DiceEventArgs();

			diceEventArgs.SetDiceData(diceData);
			OnDiceStoppedRolling(null, diceEventArgs);
		}
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
								hubConnection.On<string>("DiceHaveStoppedRolling", DiceHaveStoppedRolling);
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

		public static void ChangePlayerHealth(string playerData)
		{
			HubConnection.InvokeAsync("ChangePlayerHealth", playerData);
		}

		public static void AddWindup(string windupData)
		{
			HubConnection.InvokeAsync("AddWindup", windupData);
		}

		public static void ClearWindup(string windupName)
		{
			HubConnection.InvokeAsync("ClearWindup", windupName);
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

		public static void UpdateClock(string clockData)
		{
			HubConnection.InvokeAsync("UpdateClock", clockData);
		}

		public static void RollDice(string diceData)
		{
			HubConnection.InvokeAsync("RollDice", diceData);
		}

		public static void ClearDice()
		{
			HubConnection.InvokeAsync("ClearDice");
		}

		public static void SetPlayerData(string playerData)
		{
			HubConnection.InvokeAsync("SetPlayerData", playerData);
		}
		public static void SendScrollLayerCommand(string commandData)
		{
			HubConnection.InvokeAsync("SendScrollLayerCommand", commandData);
		}
	}
}
