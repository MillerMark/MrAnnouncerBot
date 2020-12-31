using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace DHDM
{
	public enum CardCommandType
	{
		ToggleHandVisibility,
		HideAllNpcCards,
		SelectNextCard,
		SelectPreviousCard,
		PlaySelectedCard
	}
	
	public class CardCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		const string STR_ToggleHandVisibility = "ToggleHandVisibility ";
		const string STR_HideAllNpcCards = "HideAllNpcCards";
		const string STR_SelectNextNpcCard = "SelectNextNpcCard";
		const string STR_SelectPreviousNpcCard = "SelectPreviousNpcCard";
		const string STR_PlaySelectedNpcCard = "PlaySelectedNpcCard";
		CardCommandType cardCommandType;

		int creatureId;
		int lastNpcCreatureId;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.CardCommand(cardCommandType, creatureId);
		}

		int GetCreatureId(string idStr)
		{
			if (int.TryParse(idStr.Trim(), out int result))
				return result;
			return int.MinValue;
		}

		public bool Matches(string message)
		{
			if (message.StartsWith(STR_ToggleHandVisibility))
			{
				cardCommandType = CardCommandType.ToggleHandVisibility;
				string creatureIdStr = message.Substring(STR_ToggleHandVisibility.Length);
				creatureId = GetCreatureId(creatureIdStr);
				if (creatureId == int.MinValue)
				{
					System.Diagnostics.Debugger.Break();
					return false;
				}
				if (creatureId < 0)
					lastNpcCreatureId = creatureId;
				return true;
			}

			// TODO: Flesh this out.
			if (message == STR_HideAllNpcCards)
			{
				cardCommandType = CardCommandType.HideAllNpcCards;
				creatureId = 0;
				return true;
			}

			if (message == STR_SelectNextNpcCard)
			{
				cardCommandType = CardCommandType.SelectNextCard;
				creatureId = lastNpcCreatureId;
				return true;
			}

			if (message == STR_SelectPreviousNpcCard)
			{
				cardCommandType = CardCommandType.SelectPreviousCard;
				creatureId = lastNpcCreatureId;
				return true;
			}
			if (message == STR_PlaySelectedNpcCard)
			{
				cardCommandType = CardCommandType.PlaySelectedCard;
				creatureId = lastNpcCreatureId;
				return true;
			}

			return false;
		}
	}
}
