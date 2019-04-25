using Microsoft.AspNetCore.SignalR;
using System;
using BotCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace OverlayManager.Hubs
{
	/// <summary>
	/// Commands we send down to the Overlay.
	/// </summary>
	public interface IOverlayCommands
	{
		Task ExecuteCommand(string command, string args, string userId, string userName, string displayName, string color);
		Task UserHasCoins(string userID, int amount);
		Task PlayerDataChanged(int playerID, int pageID, string playerData);
		Task FocusItem(int playerID, int pageID, string itemID);
		Task UnfocusItem(int playerID, int pageID, string itemID);
		Task TriggerEffect(string effectData);
		Task UpdateClock(string clockData);
		Task RollDice(string diceRollData);
	}
}
