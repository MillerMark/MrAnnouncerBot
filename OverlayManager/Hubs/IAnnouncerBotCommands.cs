using System;
using System.Linq;
using System.Threading.Tasks;

namespace OverlayManager.Hubs
{
	public interface IAnnouncerBotCommands
	{
		Task AddCoins(string userID, int amount);
		Task NeedToGetCoins(string userID);
		Task DiceHaveStoppedRolling(string diceData);
		Task TellDM(string message);
		Task AllDiceHaveBeenDestroyed(string diceData);
		Task InGameUIResponse(string response);

		Task Arm(string userID);
		Task Disarm(string userID);
		Task Fire(string userID);

		Task ChangeScene(string sceneName);

		Task UserHasCoins(string userID, int amount);  // A -> B -> C
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
		Task ClearWindup(string windupName);
		Task MoveFred(string movement);
		Task AddWindup(string windupData);
		Task CastSpell(string spellData);
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
		Task SendScrollLayerCommand(string commandData);
		Task ExecuteSoundCommand(string commandData);
		Task ShowValidationIssue(string commandData);

	}
}
