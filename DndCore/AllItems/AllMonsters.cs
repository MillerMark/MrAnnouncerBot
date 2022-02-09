using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace DndCore
{
	public static class AllMonsters
	{
		public static void Invalidate()
		{
			monsters = null;
		}

		static List<Monster> monsters;
		public static void LoadData()
		{
			monsters = new List<Monster>();
			monsters.Clear();
			List<MonsterDto> monsterDtos = LoadData(Folders.InCoreData("DnD - Monsters.csv"));
			foreach (var monsterDto in monsterDtos)
			{
				monsters.Add(Monster.From(monsterDto));
			}
		}

		public static List<MonsterDto> LoadData(string dataFile)
		{
			return CsvToSheetsHelper.Get<MonsterDto>(dataFile);
		}

		public static Monster GetByKind(string monsterKind)
		{
			return Monsters.FirstOrDefault(x => x.Kind.StartsWith(monsterKind));
		}

		public static List<Monster> Monsters
		{
			get
			{
				if (monsters == null)
					LoadData();
				return monsters;
			}
			private set => monsters = value;
		}
	}
}
