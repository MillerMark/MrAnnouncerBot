﻿using System;
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
				SpellEffects.Add(ItemEffect.From(itemEffect));
			}
		}

		static List<ItemEffect> spellEffects;
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
			return SpellEffects.Where(x => x.name == spellName).OrderBy(o => o.index).ToList();
		}

		public static void Invalidate()
		{
			spellEffects = null;
		}
	}
}
