using System;
using System.Linq;
using System.Threading.Tasks;

namespace OverlayManager.Hubs
{
	public interface IAnnouncerBotCommands
	{
		Task AddCoins(string userID, int amount);
		Task NeedToGetCoins(string userID);
		Task UserHasCoins(string userID, int amount);  // A -> B -> C

		Task PlayerPageChanged(int playerID, int pageID, string playerData);
		Task FocusItem(int playerID, int pageID, string itemID);
	}
}
