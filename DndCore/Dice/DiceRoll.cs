using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	//+ Any changes here should be reflected in getDiceRollData inside DiceLayers.ts
	public class DiceRoll
	{
		public List<PlayerRollOptions> PlayerRollOptions = new List<PlayerRollOptions>();
		public List<TrailingEffect> TrailingEffects = new List<TrailingEffect>();

		public string OnFirstContactEffect { get; set; }

		public string SpellName { get; set; }
		public int NumHalos { get; set; }
		public string OnFirstContactSound { get; set; }
		public string OnRollSound { get; set; }

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
		public string AdditionalDiceOnHit { get; set; }
		public string AdditionalDiceOnHitMessage { get; set; }
		public string FailMessage { get; set; }
		public string OnThrowSound { get; set; }
		public double HiddenThreshold { get; set; }
		public bool IsMagic { get; set; }
		
		// TODO: Remove Kind...
		public VantageKind VantageKind { get; set; }
		
		public int MinCrit { get; set; }
		public double Modifier { get; set; }
		public string SuccessMessage { get; set; }
		public double ThrowPower { get; set; }
		public DiceRollType Type { get; set; }
		public RollScope RollScope { get; set; }
		public Skills SkillCheck { get; set; }
		public Ability SavingThrow { get; set; }
		public DamageType DamageType { get; set; }
		public string SecondRollTitle { get; set; }
		public int MinDamage { get; set; }
		public string EffectHueShift { get; set; }
		public int EffectSaturation { get; set; }
		public int EffectBrightness { get; set; }
		public int EffectRotation { get; set; }
		public double EffectScale { get; set; }
		public string OnStopRollingSound { get; set; }
		public void AddPlayer(int playerId, VantageKind vantageKind, string inspiration)
		{
			PlayerRollOptions.Add(new PlayerRollOptions(playerId, vantageKind, inspiration));
		}

		void AddTrailingEffect(string effect)
		{
			TrailingEffect trailingEffect = AllTrailingEffects.Get(effect);
			if (trailingEffect == null)
				return;

			TrailingEffects.Add(trailingEffect);
		}

		public void AddTrailingEffects(string dieRollEffects)
		{
			if (string.IsNullOrWhiteSpace(dieRollEffects))
				return;
			string[] effects = dieRollEffects.Split(';');
			foreach (string effect in effects)
			{
				AddTrailingEffect(effect);
			}
		}

		void AddDieRollEffect(string effect)
		{
			DieRollEffect dieRollEffect = AllDieRollEffects.Get(effect);
			if (dieRollEffect == null)
				return;
			NumHalos = dieRollEffect.NumHalos;
			EffectRotation = dieRollEffect.Rotation;
			EffectScale = dieRollEffect.Scale;
			EffectBrightness = dieRollEffect.Brightness;
			EffectSaturation = dieRollEffect.Saturation;
			EffectHueShift = dieRollEffect.HueShift;
			OnFirstContactEffect = dieRollEffect.OnFirstContactEffect;
			OnFirstContactSound = dieRollEffect.OnFirstContactSound;
			OnThrowSound = dieRollEffect.OnThrowSound;
			OnStopRollingSound = dieRollEffect.OnStopRollingSound;
		}

		public void AddDieRollEffects(string dieRollEffects)
		{
			if (string.IsNullOrWhiteSpace(dieRollEffects))
				return;
			string[] effects = dieRollEffects.Split(';');
			foreach (string effect in effects)
			{
				AddDieRollEffect(effect);
			}
		}
		public void Modify(PlayerActionShortcut actionShortcut)
		{
			
		}
		public static DiceRoll GetFrom(PlayerActionShortcut actionShortcut)
		{
			
		}
		public bool IsOnePlayer
		{
			get
			{
				return PlayerRollOptions == null || PlayerRollOptions.Count == 1;
			}
		}
	}
}
