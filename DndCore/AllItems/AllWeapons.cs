using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllWeapons
	{
		static List<Weapon> weapons = new List<Weapon>();
		static AllWeapons()
		{
			LoadData();
		}

		public static void LoadData()
		{
			weapons.Clear();
			List<WeaponDto> weaponDtos = LoadData(Folders.InCoreData("DnD - Weapons.csv"));
			foreach (var weaponDto in weaponDtos)
			{
				weapons.Add(Weapon.From(weaponDto));
			}
		}

		public static List<WeaponDto> LoadData(string dataFile)
		{
			return CsvData.Get<WeaponDto>(dataFile);
		}

		public static Weapon Get(string weaponName)
		{
			return Weapons.FirstOrDefault(x => x.Name.StartsWith(weaponName));
		}

		public static List<Weapon> Weapons { get => weapons; private set => weapons = value; }
	}
}
