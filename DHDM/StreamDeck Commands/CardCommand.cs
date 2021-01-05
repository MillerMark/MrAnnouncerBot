using System;
using DndCore;
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
		const string STR_SelectNextPlayerCard = "SelectNextPlayerCard";
		const string STR_SelectPreviousNpcCard = "SelectPreviousNpcCard";
		const string STR_SelectPreviousPlayerCard = "SelectPreviousPlayerCard";
		const string STR_PlaySelectedNpcCard = "PlaySelectedNpcCard";
		CardCommandType cardCommandType;

		int creatureId;
		int lastNpcCreatureId;
		string playerName;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			SetCreatureId(dungeonMasterApp);
			if (creatureId == int.MinValue && playerName != null)
			{
				int playerId = dungeonMasterApp.GetPlayerIdFromName(playerName);
				dungeonMasterApp.CardCommand(cardCommandType, playerId);
			}
			else
				dungeonMasterApp.CardCommand(cardCommandType, creatureId);
		}

		private void SetCreatureId(IDungeonMasterApp dungeonMasterApp)
		{
			if (targetOverride != null)
			{
				if (targetOverride == "{lastNpcCreatureId}")
					creatureId = lastNpcCreatureId;
				else
				{
					int playerId = dungeonMasterApp.GetPlayerIdFromName(targetOverride);
					if (playerId >= 0)
						creatureId = playerId;
					else
					{
						System.Diagnostics.Debugger.Break();
					}
				}
			}
		}

		int GetCreatureId(string idStr)
		{
			if (int.TryParse(idStr.Trim(), out int result))
				return result;
			return int.MinValue;
		}

		string targetOverride;
		public bool Matches(string message)
		{
			targetOverride = null;
			playerName = null;
			if (message.StartsWith(STR_ToggleHandVisibility))
			{
				cardCommandType = CardCommandType.ToggleHandVisibility;
				string creatureIdStr = message.Substring(STR_ToggleHandVisibility.Length);
				creatureId = GetCreatureId(creatureIdStr);
				if (creatureId == int.MinValue)
				{
					playerName = creatureIdStr;
					return true;
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

			if (message.StartsWith(STR_SelectNextNpcCard) || message.StartsWith(STR_SelectNextPlayerCard))
			{
				targetOverride = message.EverythingAfter(" ").Trim();
				cardCommandType = CardCommandType.SelectNextCard;
				return true;
			}

			if (message.StartsWith(STR_SelectPreviousNpcCard) || message.StartsWith(STR_SelectPreviousPlayerCard))
			{
				targetOverride = message.EverythingAfter(" ").Trim();
				cardCommandType = CardCommandType.SelectPreviousCard;
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
