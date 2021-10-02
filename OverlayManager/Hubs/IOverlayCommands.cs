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
		Task DmDataChanged(string dmData);
		Task CalibrateLeapMotion(string calibrationData);
		Task UpdateSkeletalData(string skeletalData);
		Task SpeechBubble(string speechStr);
		Task CardCommand(string cardStr);
		Task ContestCommand(string contestStr);
		Task ChangePlayerHealth(string playerData);
		Task ChangePlayerStats(string playerStatsData);
		Task ChangePlayerWealth(string playerData);
		Task ChangeFrameRate(string frameRateData);
		Task InGameUICommand(string commandData);
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
		Task ClearDice(string diceGroup);
		Task SetPlayerData(string playerData);
		Task ShowImageFront(string fileName);
		Task ShowImageBack(string fileName);
		Task SendScrollLayerCommand(string commandData);
		Task ExecuteSoundCommand(string commandData);
		Task ShowValidationIssue(string commandData);
		Task PreloadImageBack(string baseFileName, int indexStart, int indexStop, int digitCount);
		Task PreloadImageFront(string baseFileName, int indexStart, int indexStop, int digitCount);
	}
}
