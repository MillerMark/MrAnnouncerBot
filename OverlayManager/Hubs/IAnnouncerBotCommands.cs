using System;
using System.Linq;
using System.Threading.Tasks;

namespace OverlayManager.Hubs
{
	public interface IAnnouncerBotCommands
	{
		Task AddCoins(string userID, int amount);
		Task NeedToGetCoins(string userID);

		Task Arm(string userID);
		Task Disarm(string userID);
		Task Fire(string userID);

		Task ChangeScene(string sceneName);

		Task UserHasCoins(string userID, int amount);  // A -> B -> C

		Task PlayerDataChanged(int playerID, int pageID, string playerData);
		Task FocusItem(int playerID, int pageID, string itemID);
		Task UnfocusItem(int playerID, int pageID, string itemID);
		Task TriggerEffect(string effectData);
		Task RollDice(string diceRollData);

	}
}
