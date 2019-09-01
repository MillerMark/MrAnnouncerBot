using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllPlayers
	{
		static List<Character> players = new List<Character>();
		static AllPlayers()
		{
			LoadData();
		}

		public static void LoadData()
		{
			players.Clear();
			List<CharacterDto> playerDtos = LoadData(Folders.InCoreData("DnD - Players.csv"));
			foreach (var characterDto in playerDtos)
			{
				players.Add(Character.From(characterDto));
			}
		}

		public static List<CharacterDto> LoadData(string dataFile)
		{
			return CsvData.Get<CharacterDto>(dataFile);
		}

		public static Character Get(string playerName)
		{
			return Players.FirstOrDefault(x => x.name.StartsWith(playerName));
		}

		public static List<Character> GetActive()
		{
			return Players.Where(x => x.playingNow).OrderBy(x => x.leftMostPriority).ToList();
		}

		public static int GetPlayerIdFromNameStart(List<Character> players, string characterName)
		{
			string lowerName = characterName.ToLower();
			foreach (Character character in players)
			{
				if (character.name.ToLower().StartsWith(lowerName))
				{
					return character.playerID;
				}
			}
			return -1;
		}
		public static Character GetFromId(int playerId)
		{
			return Players.FirstOrDefault(x => x.playerID == playerId);
		}

		public static List<Character> Players { get => players; private set => players = value; }
	}
}
