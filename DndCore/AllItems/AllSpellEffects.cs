using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllSpellEffects
	{
		static AllSpellEffects()
		{
			SpellEffects = new List<ItemEffect>();
			LoadData();
		}

		public static void LoadData()
		{
			SpellEffects.Clear();
			List<ItemEffectDto> spellEffectDtos = CsvData.Get<ItemEffectDto>(Folders.InCoreData("DnD - SpellEffects.csv"), false);
			foreach (ItemEffectDto itemEffect in spellEffectDtos)
			{
				SpellEffects.Add(ItemEffect.From(itemEffect));
			}
		}

		public static List<ItemEffect> SpellEffects { get; private set; }

		public static List<ItemEffect> GetAll(string spellName)
		{
			return SpellEffects.Where(x => x.name == spellName).OrderBy(o => o.index).ToList();
		}
	}
}
