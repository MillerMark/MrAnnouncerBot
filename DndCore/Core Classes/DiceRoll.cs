using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class DiceRoll
	{
		public List<TrailingEffect> TrailingEffects = new List<TrailingEffect>();

		public DiceRoll(DiceRollKind kind = DiceRollKind.Normal, string damageDice = "")
		{
			DamageDice = damageDice;
			Kind = kind;
			ThrowPower = 1.0;
			MinCrit = 20;  // Some feats allow for a 19 to crit.
			SuccessMessage = "Success!";
			FailMessage = "Fail!";
			CritFailMessage = "Critical Fail!";
			CritSuccessMessage = "Critical Success!";
		}

		public string CritFailMessage { get; set; }
		public string CritSuccessMessage { get; set; }
		public string DamageDice { get; set; }
		public string Inspiration { get; set; }
		public string FailMessage { get; set; }
		public double HiddenThreshold { get; set; }
		public bool IsMagic { get; set; }
		public bool IsPaladinSmiteAttack { get; set; }
		public bool IsSneakAttack { get; set; }
		public bool IsWildAnimalAttack { get; set; }
		public DiceRollKind Kind { get; set; }
		public int MinCrit { get; set; }
		public double Modifier { get; set; }
		public int NumHalos { get; set; }
		public TrailingSpriteType OnFirstContactEffect { get; set; }
		public string OnFirstContactSound { get; set; }
		public string OnRollSound { get; set; }
		public string SuccessMessage { get; set; }
		public double ThrowPower { get; set; }
		public DiceRollType Type { get; set; }
		public RollScope RollScope { get; set; }
		public int IndividualFilter { get; set; }
		public Skills SkillCheck { get; set; }
		public Ability SavingThrow { get; set; }
	}
}
