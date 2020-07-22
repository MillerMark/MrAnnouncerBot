using DndCore;
using DndUI;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DHDM
{
	public static class HubtasticBaseStation
	{
		public delegate void DiceEventHandler(object sender, DiceEventArgs ea);
		public delegate void MessageEventHandler(object sender, MessageEventArgs ea);

		static readonly object hubConnectionLock = new object();
		static HubConnection hubConnection;
		public static event DiceEventHandler DiceStoppedRolling;
		public static event MessageEventHandler TellDungeonMaster;
		public static event DiceEventHandler AllDiceDestroyed;
		public static void OnDiceStoppedRolling(object sender, DiceEventArgs ea)
		{
			DiceStoppedRolling?.Invoke(sender, ea);
		}
		public static void OnTellDM(object sender, MessageEventArgs ea)
		{
			TellDungeonMaster?.Invoke(sender, ea);
		}

		public static void OnAllDiceDestroyed(object sender, DiceEventArgs ea)
		{
			AllDiceDestroyed?.Invoke(sender, ea);
		}

		static DiceEventArgs diceEventArgs;
		static void DiceHaveStoppedRolling(string diceData)
		{
			if (diceEventArgs == null)
				diceEventArgs = new DiceEventArgs();

			diceEventArgs.SetDiceData(diceData);
			OnDiceStoppedRolling(null, diceEventArgs);
		}
		static void TellTheDungeonMaster(string message)
		{
			OnTellDM(null, new MessageEventArgs(message));
		}
		static void AllDiceHaveBeenDestroyed(string diceData)
		{
			if (diceEventArgs == null)
				diceEventArgs = new DiceEventArgs();

			diceEventArgs.SetDiceData(diceData);
			OnAllDiceDestroyed(null, diceEventArgs);
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
								hubConnection.Closed += HubConnection_Closed;
								// TODO: Check out benefits of stopping gracefully with a cancellation token.
								hubConnection.On<string>("DiceHaveStoppedRolling", DiceHaveStoppedRolling);
								hubConnection.On<string>("AllDiceHaveBeenDestroyed", AllDiceHaveBeenDestroyed);
								hubConnection.On<string>("TellDM", TellTheDungeonMaster);
								hubConnection.StartAsync();
							}
						}
					}
				}
				
				return hubConnection;
			}
		}

		private static Task HubConnection_Closed(Exception arg)
		{
			return Task.CompletedTask;
		}

		public static void PlayerDataChanged(int playerID, ScrollPage pageID, string playerData)
		{
			HubConnection.InvokeAsync("PlayerDataChanged", playerID, (int)pageID, playerData);
		}

		public static void PlayerDataChanged(int playerID, string playerData)
		{
			HubConnection.InvokeAsync("PlayerDataChanged", playerID, -1, playerData);
		}

		public static void MapDataChanged(string mapData)
		{
			HubConnection.InvokeAsync("MapDataChanged", mapData);
		}

		public static void ChangePlayerHealth(string playerData)
		{
			HubConnection.InvokeAsync("ChangePlayerHealth", playerData);
		}

		public static void ChangePlayerWealth(string playerData)
		{
			HubConnection.InvokeAsync("ChangePlayerWealth", playerData);
		}

		public static void ChangeFrameRate(string frameRateData)
		{
			HubConnection.InvokeAsync("ChangeFrameRate", frameRateData);
		}

		public static void AddWindup(string windupData)
		{
			HubConnection.InvokeAsync("AddWindup", windupData);
		}

		public static void CastSpell(string spellData)
		{
			HubConnection.InvokeAsync("CastSpell", spellData);
		}

		public static void ClearWindup(string windupName)
		{
			HubConnection.InvokeAsync("ClearWindup", windupName);
		}

		public static void MoveFred(string movement)
		{
			HubConnection.InvokeAsync("MoveFred", movement);
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

		public static void PlaySound(string soundFileName)
		{
			HubConnection.InvokeAsync("PlaySound", soundFileName);
		}

		public static void AnimateSprinkles(string commandData)
		{
			HubConnection.InvokeAsync("AnimateSprinkles", commandData);
		}

		// TODO: Call this to send in game creatures over SignalR to the TS overlay.
		public static void UpdateInGameCreatures(string command, List<InGameCreature> creatures)
		{
			InGameCommand inGameCommand = new InGameCommand(command, creatures);
			string commandData = JsonConvert.SerializeObject(inGameCommand);
			HubConnection.InvokeAsync("UpdateInGameCreatures", commandData);
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
		public static void ExecuteSoundCommand(string commandData)
		{
			HubConnection.InvokeAsync("ExecuteSoundCommand", commandData);
		}
		public static void FloatPlayerText(int playerID, string message, string fillColor, string outlineColor)
		{
			HubConnection.InvokeAsync("FloatPlayerText", playerID, message, fillColor, outlineColor);
		}
	}
}
