using System;
using System.Linq;
using System.Threading.Tasks;

namespace OverlayManager.Hubs
{
	public interface IAnnouncerBotCommands
	{
		Task AddCoins(string userID, int amount);
		Task NeedToGetCoins(string userID);
		Task UserHasCoins(string userID, int amount);
	}
}
