using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;
using System.IO;

namespace DndCore
{
	public static class AllSpellEffects
	{
		static void LoadData()
		{
			spellEffects = new List<ItemEffect>();
			List<ItemEffectDto> spellEffectDtos = CsvToSheetsHelper.Get<ItemEffectDto>(Folders.InCoreData("DnD - SpellEffects.csv"));
			foreach (ItemEffectDto itemEffect in spellEffectDtos)
			{
				spellEffects.Add(ItemEffect.From(itemEffect));
			}
			spellEffectsByName = spellEffects
				.GroupBy(e => e.name)
				.ToDictionary(g => g.Key, g => g.OrderBy(e => e.index).ToList());
		}

		static List<ItemEffect> spellEffects;
		static Dictionary<string, List<ItemEffect>> spellEffectsByName;

		public static List<ItemEffect> SpellEffects
		{
			get
			{
				if (spellEffects == null)
					LoadData();
				return spellEffects;
			}
			private set
			{
				spellEffects = value;
			}
		}

		public static List<ItemEffect> GetAll(string spellName)
		{
			if (spellEffects == null)
				LoadData();
			if (spellEffectsByName != null && spellEffectsByName.TryGetValue(spellName, out List<ItemEffect> cached))
				return cached;
			return new List<ItemEffect>();
		}

		public static void Invalidate()
		{
			spellEffects = null;
			spellEffectsByName = null;
		}
	}
}
