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
		Task ExecuteCommand(string command, string args, UserInfo userInfo);
		Task UserHasCoins(string userID, int amount);
		Task SuppressVolume(int seconds);
		Task PlayerDataChanged(int playerID, int pageID, string playerData);
		Task MapDataChanged(string mapData);
		Task ChangePlayerHealth(string playerData);
		Task ChangePlayerStats(string playerStatsData);
		Task ChangePlayerWealth(string playerData);
		Task ChangeFrameRate(string frameRateData);
		Task AddWindup(string windupData);
		Task CastSpell(string spellData);
		Task ClearWindup(string windupName);
		Task MoveFred(string movement);
		Task FocusItem(int playerID, int pageID, string itemID);
		Task UnfocusItem(int playerID, int pageID, string itemID);
		Task TriggerEffect(string effectData);
		Task PlaySound(string soundFileName);
		Task AnimateSprinkles(string commandData);
		Task UpdateInGameCreatures(string commandData);
		Task UpdateClock(string clockData);
		Task FloatPlayerText(int playerID, string message, string fillColor, string outlineColor);
		Task RollDice(string diceRollData);
		Task ClearDice();
		Task SetPlayerData(string playerData);
		Task SendScrollLayerCommand(string commandData);
		Task ExecuteSoundCommand(string commandData);
	}
}
