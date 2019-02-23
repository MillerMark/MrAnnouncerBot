using System;
using System.Linq;

namespace DndCore
{
	public class Mod
	{
		public ModType type = ModType.playerProperty;
		public string targetName;  // e.g., target property name.
		public bool requiresEquipped;
		public bool requiresConsumption;
		public DamageFilter damageTypeFilter = null;
		public Conditions condition = Conditions.None;
		public double offset = 0;
		public double multiplier = 1; // < 1 for resistance, > 1 for vulnerability
		public DndTimeSpan repeats = DndTimeSpan.Never;
		public DateTime lastApplied = DateTime.MinValue;

		public Mod()
		{

		}
	}
}
