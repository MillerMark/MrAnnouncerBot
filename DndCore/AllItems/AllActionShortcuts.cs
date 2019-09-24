using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllActionShortcuts
	{
		static List<PlayerActionShortcut> shortcuts = new List<PlayerActionShortcut>();
		static AllActionShortcuts()
		{
			LoadData();
		}

		static void ProcessDtos(List<PlayerActionShortcutDto> playerDtos)
		{
			shortcuts.Clear();

			int index = 0;
			List<PlayerActionShortcut> lastShortcuts = null;
			while (index <= playerDtos.Count - 1)
			{
				PlayerActionShortcutDto thisShortcutDto = playerDtos[index];

				if (string.IsNullOrWhiteSpace(thisShortcutDto.player)) // appending to last entry...
				{
					foreach (PlayerActionShortcut shortcut in lastShortcuts)
					{
						Character player = AllPlayers.GetFromId(shortcut.PlayerId);

						bool isWindup = shortcut.Spell != null && !shortcut.Spell.Duration.HasValue() && shortcut.Spell.MustRollDiceToCast();
						shortcut.AddEffect(thisShortcutDto, shortcut.lastPrefix, player, shortcut.SpellSlotLevel, isWindup);
					}
				}
				else
				{
					lastShortcuts = PlayerActionShortcut.From(thisShortcutDto);
					foreach (PlayerActionShortcut shortcut in lastShortcuts)
					{
						shortcuts.Add(shortcut);
					}
				}
				index++;
			}
		}
		public static void LoadData()
		{
			ProcessDtos(LoadData(Folders.InCoreData("DnD - Shortcuts.csv")));
		}

		public static List<PlayerActionShortcutDto> LoadData(string dataFile)
		{
			return CsvData.Get<PlayerActionShortcutDto>(dataFile);
		}

		public static List<PlayerActionShortcut> Get(int playerId)
		{
			return AllShortcuts.Where(x => x.PlayerId == playerId).ToList();
		}
		
		public static List<PlayerActionShortcut> Get(int playerId, TurnPart part)
		{
			return AllShortcuts.Where(x => x.PlayerId == playerId).Where(x => x.Part == part).ToList();
		}

		public static List<PlayerActionShortcut> Get(int playerId, string nameStart)
		{
			string lowerName = nameStart.ToLower();
			return AllShortcuts.Where(x => x.PlayerId == playerId).Where(x => x.Name.ToLower().StartsWith(lowerName)).ToList();
		}

		public static List<PlayerActionShortcut> AllShortcuts { get => shortcuts; private set => shortcuts = value; }
	}
}

