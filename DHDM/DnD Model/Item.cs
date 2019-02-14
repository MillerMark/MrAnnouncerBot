using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHDM
{
	public class Item
	{
		public DndTimeSpan EquipTime = DndTimeSpan.Zero;
		public DndTimeSpan UnequipTime = DndTimeSpan.Zero;
		public string name;
		public List<Mod> mods = new List<Mod>();
		public List<Attack> attacks = new List<Attack>();
		public List<CurseBlessingDisease> cursesBlessingsDiseases = new List<CurseBlessingDisease>();
		public bool consumable;
		public int weight = 0;
		public int costValue;
		public bool magic;
		public bool equipped;
		public bool silvered;
		public bool adamantine;
		public int count = 1;
		public string description = string.Empty;
		public List<Effect> consumedEffects;
		public List<Effect> equippedEffects;
		public List<Effect> unequippedEffects;
		public List<Effect> discardedEffects;
	}
}
