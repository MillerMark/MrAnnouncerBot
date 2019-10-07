using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllWeaponEffects
	{
		static AllWeaponEffects()
		{
			WeaponEffects = new List<ItemEffect>();
			LoadData();
		}

		public static void LoadData()
		{
			WeaponEffects.Clear();
			List<ItemEffectDto> weaponEffectDtos = CsvData.Get<ItemEffectDto>(Folders.InCoreData("DnD - WeaponEffects.csv"), false);
			foreach (ItemEffectDto itemEffect in weaponEffectDtos)
			{
				WeaponEffects.Add(ItemEffect.From(itemEffect));
			}
		}

		public static List<ItemEffect> WeaponEffects { get; private set; }

		public static List<ItemEffect> GetAll(string weaponName)
		{
			return WeaponEffects.Where(x => x.name == weaponName).OrderBy(o => o.index).ToList();
		}
	}
}
