using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace Streamloots
{
	public class CardDto: CardBaseCommandDto
	{
		public const string CMD_PlayCardNow = "PlayCardNow";
		public const string CMD_ShowCard = "ShowCard";
		public const string CMD_ShowPurchase = "ShowPurchase";
		public string InstanceID { get; set; }
		public StreamlootsPurchase Purchase { get; set; }
		public StreamlootsCard Card { get; set; }
		public int OwningCharacterId { get; set; }
		public int TargetCharacterId { get; set; }

		public static string GetFileName(string activeCardName)
		{
			string fileName = activeCardName;
			foreach (char c in System.IO.Path.GetInvalidFileNameChars())
				fileName = fileName.Replace(c, '_');
			fileName = fileName.Replace(' ', '_');  // Remove spaces.
			fileName = fileName.Replace("+", "Plus");  // Remove + character.
			return fileName;
		}

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
			InstanceID = Guid.NewGuid().ToString();
			Card = card;
			OwningCharacterId = GetCreatureId(card.Recipient);
			TargetCharacterId = GetCreatureId(card.Target);
			if (TargetCharacterId != int.MinValue)
				Command = CMD_PlayCardNow;
			else
				Command = CMD_ShowCard;
		}

		public CardDto(StreamlootsPurchase purchase)
		{
			InstanceID = Guid.NewGuid().ToString();
			Purchase = purchase;
			Command = CMD_ShowPurchase;
		}
		public CardDto()
		{

		}
	}
}
