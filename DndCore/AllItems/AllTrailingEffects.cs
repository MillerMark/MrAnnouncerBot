using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllTrailingEffects
	{
		static AllTrailingEffects()
		{
			TrailingEffects = new List<TrailingEffect>();
			LoadData();
		}

		public static void LoadData()
		{
			TrailingEffects.Clear();
			List<TrailingEffectsDto> trailingEffectsDtos = CsvData.Get<TrailingEffectsDto>(Folders.InCoreData("DnD - TrailingEffects.csv"), false);
			foreach (TrailingEffectsDto trailingEffect in trailingEffectsDtos)
			{
				TrailingEffects.Add(TrailingEffect.From(trailingEffect));
			}
		}

		public static List<TrailingEffect> TrailingEffects { get; private set; }

		public static TrailingEffect Get(string effectName)
		{
			return TrailingEffects.FirstOrDefault(x => x.Name == effectName);
		}
	}
}
