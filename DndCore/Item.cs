using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DndCore
{
	public class Item
	{
		public DndTimeSpan equipTime = DndTimeSpan.Zero;
		public DndTimeSpan unequipTime = DndTimeSpan.Zero;
		public string name = string.Empty;
		public string description = string.Empty;
		public bool consumable = false;
		public int weight = 0;
		public int costValue = 0;
		public bool magic = false;
		public bool equipped = false;
		public bool silvered = false;
		public bool adamantine = false;
		public int count = 1;
		public List<Mod> mods = new List<Mod>();
		public List<Attack> attacks = new List<Attack>();
		public List<CurseBlessingDisease> cursesBlessingsDiseases = new List<CurseBlessingDisease>();
		public List<Effect> consumedEffects = new List<Effect>();
		public List<Effect> equippedEffects = new List<Effect>();
		public List<Effect> unequippedEffects = new List<Effect>();
		public List<Effect> discardedEffects = new List<Effect>();

		public void Consume(Character owner)
		{
			if (!consumable)
				return;

			foreach (Mod mod in mods)
				if (mod.requiresConsumption)
					owner.ApplyModPermanently(mod, name);
		}

		void ApplyAllMods(Character owner)
		{
			foreach (Mod mod in mods)
				if (!mod.requiresConsumption)
					if (!mod.requiresEquipped || equipped)
						owner.ApplyModTemporarily(mod, name);
		}
	}
}
