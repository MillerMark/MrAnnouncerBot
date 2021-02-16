using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace Streamloots
{
	public class CardDto: CardBaseCommandDto, IGetUserName
	{
		public const string CMD_RollDie = "RollDie";
		public const string CMD_PlayCardNow = "PlayCardNow";
		public const string CMD_PlayCardWithTarget = "PlayCardWithTarget";
		public const string CMD_ShowCard = "ShowCard";
		public const string CMD_ShowPurchase = "ShowPurchase";
		public string InstanceID { get; set; }
		public StreamlootsPurchase Purchase { get; set; }
		public StreamlootsCard Card { get; set; }
		public int OwningCharacterId { get; set; }
		public List<int> TargetCharacterIds { get; set; }

		public static string GetFileName(string activeCardName)
		{
			string fileName = activeCardName;
			foreach (char c in System.IO.Path.GetInvalidFileNameChars())
				fileName = fileName.Replace(c, '_');
			fileName = fileName.Replace(' ', '_');  // Remove spaces.
			fileName = fileName.Replace("+", "Plus");  // Remove + character.
			return fileName;
		}

		List<int> GetCreatureIds(string targetName)
		{
			List<int> result = new List<int>();
			
			string conditionedStr = targetName.Trim().ToLower().TrimEnd('!', '?', '.');
			conditionedStr = conditionedStr.Replace(" and ", ",");
			conditionedStr = conditionedStr.Replace("&", ",");
			conditionedStr = conditionedStr.Replace(";", ",");
			conditionedStr = conditionedStr.Replace(",,", ",");
			string[] names = conditionedStr.Split(',');
			foreach (string name in names)
			{
				int creatureId = GetCreatureId(name);
				if (creatureId == int.MinValue)
				{
					if (name.IndexOf(' ') >= 0)
					{
						string[] subNames = name.Trim().Split(' ');
						foreach (string subName in subNames)
						{
							creatureId = GetCreatureId(subName);
							if (creatureId != int.MinValue)
								if (result.IndexOf(creatureId) < 0)
									result.Add(creatureId);
						}
					}
				}
				else if (result.IndexOf(creatureId) < 0)
					result.Add(creatureId);
			}

			return result;
		}

		int GetCreatureId(string targetName)
		{
			string trimmedName = targetName.Trim();
			if (string.IsNullOrWhiteSpace(trimmedName))
				return int.MinValue;
			int playerIdFromName = AllPlayers.GetPlayerIdFromName(trimmedName);
			if (playerIdFromName >= 0)
				return playerIdFromName;
			InGameCreature creatureByName = AllInGameCreatures.GetCreatureByName(trimmedName);
			if (creatureByName != null)
				return -creatureByName.Index;
			return int.MinValue;
		}

		public string GetUserName()
		{
			return Card.UserName;
		}

		public bool NeedToRollDice
		{
			get
			{
				if (Card == null)
					return false;
				return Card.message.Contains("!" + CMD_RollDie);
			}
		}
		
		public CardDto(StreamlootsCard card)
		{
			InstanceID = Guid.NewGuid().ToString();
			Card = card;
			OwningCharacterId = GetCreatureId(card.Recipient);
			TargetCharacterIds = GetCreatureIds(card.Target);
			bool hasTarget = TargetCharacterIds.Any();
			bool hasOwner = OwningCharacterId != int.MinValue;
			if (hasTarget && NeedToRollDice)
				Command = CMD_PlayCardWithTarget;
			else if (!hasTarget && !hasOwner)
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
