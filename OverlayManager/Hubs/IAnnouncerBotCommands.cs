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

		Task Arm(string userID);
		Task Disarm(string userID);
		Task Fire(string userID);

		Task ChangeScene(string sceneName);

		Task UserHasCoins(string userID, int amount);  // A -> B -> C
		Task SuppressVolume(int seconds);

		Task PlayerDataChanged(int playerID, int pageID, string playerData);
		Task ChangePlayerHealth(string playerData);
		Task ClearWindup(string windupName);
		Task MoveFred(string movement);
		Task AddWindup(string windupData);
		Task CastSpell(string spellData);
		Task FocusItem(int playerID, int pageID, string itemID);
		Task UnfocusItem(int playerID, int pageID, string itemID);
		Task TriggerEffect(string effectData);
		Task UpdateClock(string clockData);
		Task RollDice(string diceRollData);
		Task ClearDice();
		Task SetPlayerData(string playerData);
		Task SendScrollLayerCommand(string commandData);

	}
}
