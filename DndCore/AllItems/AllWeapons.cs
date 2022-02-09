using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace DndCore
{
	public static class AllWeapons
	{
		static List<Weapon> weapons;
		public static void LoadData()
		{
			weapons = new List<Weapon>();

			List<WeaponDto> weaponDtos = LoadData(Folders.InCoreData("DnD - Weapons.csv"));
			foreach (var weaponDto in weaponDtos)
			{
				weapons.Add(Weapon.From(weaponDto));
			}
		}

		public static List<WeaponDto> LoadData(string dataFile)
		{
			return CsvToSheetsHelper.Get<WeaponDto>(dataFile);
		}

		public static Weapon Get(string weaponName)
		{
			if (weaponName == null)
				return null;
			return Weapons.FirstOrDefault(x => x.Name.StartsWith(weaponName));
		}

		public static void Invalidate()
		{
			weapons = null;
		}

		public static List<Weapon> Weapons
		{
			get
			{
				if (weapons == null)
					LoadData();
				return weapons;
			}
			private set => weapons = value;
		}
	}
}
