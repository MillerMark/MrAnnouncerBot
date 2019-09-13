using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllDieRollEffects
	{
		static AllDieRollEffects()
		{
			DieRollEffects = new List<DieRollEffect>();
			LoadData();
		}

		public static void LoadData()
		{
			DieRollEffects.Clear();
			List<DieRollEffectDto> dieRollEffectDtos = CsvData.Get<DieRollEffectDto>(Folders.InCoreData("DnD - DieRollEffects.csv"), false);
			foreach (DieRollEffectDto dieRollEffect in dieRollEffectDtos)
			{
				DieRollEffects.Add(DieRollEffect.From(dieRollEffect));
			}
		}

		public static List<DieRollEffect> DieRollEffects { get; private set; }

		public static DieRollEffect Get(string effectName)
		{
			return DieRollEffects.FirstOrDefault(x => x.Name == effectName);
		}
	}
}
