using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllTrailingEffects
	{
		public static void Invalidate()
		{
			trailingEffects = null;
		}

		static void LoadData()
		{
			trailingEffects = new List<TrailingEffect>();
			List<TrailingEffectsDto> trailingEffectsDtos = CsvData.Get<TrailingEffectsDto>(Folders.InCoreData("DnD - TrailingEffects.csv"), false);
			foreach (TrailingEffectsDto trailingEffect in trailingEffectsDtos)
			{
				TrailingEffects.Add(TrailingEffect.From(trailingEffect));
			}
		}

		static List<TrailingEffect> trailingEffects;
		public static List<TrailingEffect> TrailingEffects
		{
			get
			{
				if (trailingEffects == null)
					LoadData();
				return trailingEffects;
			}
			private set
			{
				trailingEffects = value;
			}
		}
		public static TrailingEffect Get(string effectName)
		{
			return TrailingEffects.FirstOrDefault(x => x.Name == effectName);
		}
	}
}
