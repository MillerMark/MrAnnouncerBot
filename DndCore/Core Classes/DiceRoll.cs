using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class PlayerRollOptions
	{
		public int PlayerID { get; set; }
		public string Inspiration { get; set; }
		public VantageKind VantageKind { get; set; }
		public PlayerRollOptions(int playerId, VantageKind vantageKind, string inspiration)
		{
			PlayerID = playerId;
			VantageKind = vantageKind;
			Inspiration = inspiration;
		}
	}

	public class DamageHealthChange
	{
		public List<int> PlayerIds = new List<int>();
		public int DamageHealth { get; set; }
		public DamageHealthChange()
		{
			
		}
	}

	//! Any changes here should be reflected in getDiceRollData inside DiceLayers.ts
	public class DiceRoll
	{
		public List<PlayerRollOptions> PlayerRollOptions = new List<PlayerRollOptions>();
		public List<TrailingEffect> TrailingEffects = new List<TrailingEffect>();

		public DiceRoll(VantageKind kind = VantageKind.Normal, string damageDice = "")
		{
			DamageDice = damageDice;
			VantageKind = kind;
			ThrowPower = 1.0;
			MinCrit = 20;  // Some feats allow for a 19 to crit.
			SuccessMessage = "Success!";
			FailMessage = "Fail!";
			CritFailMessage = "Critical Fail!";
			CritSuccessMessage = "Critical Success!";
			MinDamage = 0;
		}

		public string CritFailMessage { get; set; }
		public string CritSuccessMessage { get; set; }
		public string DamageDice { get; set; }
		public string GroupInspiration { get; set; }
		public string FailMessage { get; set; }
		public double HiddenThreshold { get; set; }
		public bool IsMagic { get; set; }
		public bool IsPaladinSmiteAttack { get; set; }
		public bool IsSneakAttack { get; set; }
		public bool IsWildAnimalAttack { get; set; }
		
		// TODO: Remove Kind...
		public VantageKind VantageKind { get; set; }
		
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
		public Skills SkillCheck { get; set; }
		public Ability SavingThrow { get; set; }
		public DamageType DamageType { get; set; }
		public string SecondRollTitle { get; set; }
		public int MinDamage { get; set; }
		public void AddPlayer(int playerId, VantageKind vantageKind, string inspiration)
		{
			PlayerRollOptions.Add(new PlayerRollOptions(playerId, vantageKind, inspiration));
		}
	}
}
