using System;
using System.Linq;

namespace DHDM
{
	public class Mod
	{
		public ModType type = ModType.playerProperty;
		public string targetName;  // e.g., target property name.
		public bool requiresEquipped;
		public bool requiresConsumption;
		public DamageType damageTypeFilter = DamageType.None;
		public Conditions condition = Conditions.none;
		public int offset = 0;
		public int multiplier = 1; // < 1 for resistance, > 1 for vulnerability
		public DndTimeSpan repeats = DndTimeSpan.Zero;
		public DateTime lastApplied = DateTime.MinValue;

		public Mod()
		{

		}
	}
}
