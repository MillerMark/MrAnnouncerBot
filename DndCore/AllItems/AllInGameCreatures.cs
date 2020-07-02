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

		static List<InGameCreature> inGameCreatures;
		public static List<InGameCreature> Creatures
		{
			get
			{
				if (inGameCreatures == null)
					inGameCreatures = GoogleSheets.Get<InGameCreature>();
				return inGameCreatures;
			}
		}
	}
}

