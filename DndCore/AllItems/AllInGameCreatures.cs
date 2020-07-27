using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;

namespace DndCore
{
	public static class AllInGameCreatures
	{
		static AllInGameCreatures()
		{
		}
		public static void Invalidate()
		{
			inGameCreatures = null;
		}
		public static InGameCreature Get(string name)
		{
			return Creatures.FirstOrDefault(x => x.Name == name);
		}

		public static InGameCreature GetByIndex(int index)
		{
			return Creatures.FirstOrDefault(x => x.Index == index);
		}

		public static void SaveHp()
		{
			// TODO: Optimize this code to be more efficient (takes about 6 seconds).
			GoogleSheets.SaveChanges(Creatures.ToArray(), "TempHitPointsStr,HitPointsStr");
		}

		static List<InGameCreature> inGameCreatures;
		public static List<InGameCreature> Creatures
		{
			get
			{
				if (inGameCreatures == null)
					inGameCreatures = GoogleSheets.Get<InGameCreature>().Where(x => !string.IsNullOrEmpty(x.Kind)).ToList();
				return inGameCreatures;
			}
		}
	}
}

