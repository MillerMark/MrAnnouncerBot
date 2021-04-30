﻿using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;

namespace DndCore
{
	public static class AllPlayers
	{
		public static void Invalidate()
		{
			players = null;
		}

		static List<Character> players;
		public static void LoadData()
		{
			players = new List<Character>();
			players.Clear();
			List<CharacterDto> playerDtos = LoadData(Folders.InCoreData("DnD - Players.csv"));
			foreach (var characterDto in playerDtos)
			{
				players.Add(Character.From(characterDto));
			}
		}

		public static List<CharacterDto> LoadData(string dataFile)
		{
			return GoogleSheets.Get<CharacterDto>(dataFile);
		}

		public static Character Get(string playerName)
		{
			return Players.FirstOrDefault(x => x.name.StartsWith(playerName));
		}

		public static List<Character> GetActive()
		{
			return Players.Where(x => x.playingNow).OrderBy(x => x.leftMostPriority).ToList();
		}

		public static int GetPlayerIdFromName(List<Character> players, string characterName)
		{
			if (characterName == null)
				return -1;
			if (characterName.ToLower() == "zephyr")
				return 100;

			foreach (Character character in players)
				if (character.playerShortcut == characterName && character.playingNow)
					return character.playerID;

			if (characterName == "c")
				characterName = "L'il Cutie";

			string lowerName = characterName.ToLower();
			if (lowerName == "ava")
				lowerName = "l'il cutie";

			if (lowerName == "cutie")
				lowerName = "l'il cutie";

			foreach (Character character in players)
			{
				if (character.name != null && character.name.ToLower().StartsWith(lowerName) && character.playingNow)
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

		public static Character GetFromName(string name)
		{
			int playerId = GetPlayerIdFromName(name);
			if (playerId == -1)
				return null;
			return GetFromId(playerId);
		}

		public static int GetPlayerIdFromName(string name)
		{
			return GetPlayerIdFromName(Players, name);
		}

		public static List<Character> Players
		{
			get
			{
				if (players == null)
					LoadData();
				return players;
			}
			private set => players = value;
		}

		public static void SaveChanges()
		{
			GoogleSheets.SaveChanges(Players.ToArray());
		}

		public static List<Character> GetOnScreen()
		{
			return Players.Where(x => x.IsSelected).ToList();
		}

		public static Character GetFromTaleSpireId(string iD)
		{
			return Players.FirstOrDefault(x => x.taleSpireId == iD);
		}
	}
}
