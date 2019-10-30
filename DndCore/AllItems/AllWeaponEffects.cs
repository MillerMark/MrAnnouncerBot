using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllWeaponEffects
	{
		public static void Invalidate()
		{
			weaponEffects = null;
		}

		static void LoadData()
		{
			weaponEffects = new List<ItemEffect>();
			List<ItemEffectDto> weaponEffectDtos = CsvData.Get<ItemEffectDto>(Folders.InCoreData("DnD - WeaponEffects.csv"), false);
			foreach (ItemEffectDto itemEffect in weaponEffectDtos)
			{
				WeaponEffects.Add(ItemEffect.From(itemEffect));
			}
		}

		static List<ItemEffect> weaponEffects = new List<ItemEffect>();
		public static List<ItemEffect> WeaponEffects
		{
			get
			{
				if (weaponEffects == null)
					LoadData();
				return weaponEffects;
			}
			private set
			{
				weaponEffects = value;
			}
		}
		public static List<ItemEffect> GetAll(string weaponName)
		{
			return WeaponEffects.Where(x => x.name == weaponName).OrderBy(o => o.index).ToList();
		}
	}
}
