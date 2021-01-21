using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace Streamloots
{
	public class CardDto
	{
		public StreamlootsPurchase Purchase { get; set; }
		public StreamlootsCard Card { get; set; }
		public int CharacterId { get; set; }
		public string Command { get; set; }

		int GetCreatureId(string targetName)
		{
			if (string.IsNullOrWhiteSpace(targetName))
				return int.MinValue;
			int playerIdFromName = AllPlayers.GetPlayerIdFromName(targetName);
			if (playerIdFromName >= 0)
				return playerIdFromName;
			InGameCreature creatureByName = AllInGameCreatures.GetCreatureByName(targetName);
			if (creatureByName != null)
				return -creatureByName.Index;
			return int.MinValue;
		}

		public CardDto(StreamlootsCard card)
		{
			Card = card;
			CharacterId = GetCreatureId(card.Target);
			Command = "ShowCard";
		}
		public CardDto(StreamlootsPurchase purchase)
		{
			Purchase = purchase;
			Command = "ShowPurchase";
		}
		public CardDto()
		{

		}
	}
}
