using System;
using System.Linq;
using DndCore.Enums;
using DndCore.Filters;
using DndCore.CoreClasses;

namespace DndCore
{
	public class Mod
	{

		public double absolute = 0;

		public Ability addModifier;

		public bool addsAdvantage;
		public bool addsDisadvantage;
		public Conditions condition = Conditions.None;
		public DamageFilter damageTypeFilter = null;
		public DateTime lastApplied = DateTime.MinValue;
		public int modifierLimit = 0;
		public double multiplier = 1; // < 1 for resistance, > 1 for vulnerability

		public double offset = 0;

		public DndTimeSpan repeats = DndTimeSpan.Never;
		public bool requiresConsumption;
		public bool requiresEquipped;
		public string targetName;  // e.g., target property name.
		public ModType type = ModType.playerProperty;
		public Skills vantageSkillFilter = Skills.none;

		public Mod()
		{

		}
	}
}
