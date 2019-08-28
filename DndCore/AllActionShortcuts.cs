using System;
using System.Linq;
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
			PlayerActionShortcut lastShortcut = null;
			while (index <= playerDtos.Count - 1)
			{
				PlayerActionShortcutDto thisShortcutDto = playerDtos[index];
				if (string.IsNullOrWhiteSpace(thisShortcutDto.player)) // appending to last entry...
				{
					Character player = AllPlayers.GetFromId(lastShortcut.PlayerId);
					lastShortcut.AddEffect(thisShortcutDto, player);
				}
				else
				{
					lastShortcut = PlayerActionShortcut.From(thisShortcutDto);
					shortcuts.Add(lastShortcut);
				}
				index++;
			}
		}
		public static void LoadData()
		{
			ProcessDtos(LoadData(Folders.InCoreData("DnD - Shortcuts.csv")));
		}

		static List<PlayerActionShortcutDto> LoadData(string dataFile)
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

		public static List<PlayerActionShortcut> AllShortcuts { get => shortcuts; private set => shortcuts = value; }
	}
}
