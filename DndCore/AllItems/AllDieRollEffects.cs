﻿using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace DndCore
{
	public static class AllDieRollEffects
	{
		public static void Invalidate()
		{
			dieRollEffects = null;
		}

		public static void LoadData()
		{
			dieRollEffects = new List<DieRollEffect>();
			List<DieRollEffectDto> dieRollEffectDtos = CsvToSheetsHelper.Get<DieRollEffectDto>(Folders.InCoreData("DnD - DieRollEffects.csv"));
			foreach (DieRollEffectDto dieRollEffect in dieRollEffectDtos)
			{
				DieRollEffects.Add(DieRollEffect.From(dieRollEffect));
			}
		}

		static List<DieRollEffect> dieRollEffects;
		public static List<DieRollEffect> DieRollEffects
		{
			get
			{
				if (dieRollEffects == null)
					LoadData();
				return dieRollEffects;
			}
			private set
			{
				dieRollEffects = value;
			}
		}
		public static DieRollEffect Get(string effectName)
		{
			return DieRollEffects.FirstOrDefault(x => x.Name == effectName);
		}
	}
}
