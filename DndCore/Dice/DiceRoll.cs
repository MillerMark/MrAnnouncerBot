using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	//+ Any property/field changes here should be reflected in getDiceRollData inside DiceLayers.ts
	public class DiceRoll
	{
		public List<PlayerRollOptions> PlayerRollOptions = new List<PlayerRollOptions>();
		public List<TrailingEffect> TrailingEffects = new List<TrailingEffect>();

		public string OnFirstContactEffect { get; set; }
		public List<DiceDto> DiceDtos { get; set; } = new List<DiceDto>();
		
		public string RollID { get; set; }
		public string SpellID { get; set; }
		public string Viewer { get; set; }
		public string SpellName { get; set; }
		public string DieTotalMessage { get; set; }
		public string TextOutlineColor { get; set; }
		public string TextFillColor { get; set; }
		public int NumHalos { get; set; }
		public int SingleOwnerId { get; set; } = int.MinValue;
		public string OnFirstContactSound { get; set; }
		public string OnRollSound { get; set; }

		public string CritFailMessage { get; set; }
		public string CritSuccessMessage { get; set; }
		public string DamageHealthExtraDice { get; set; }
		public string GroupInspiration { get; set; }
		public string AdditionalDiceOnHit { get; set; }
		public DiceGroup DiceGroup { get; set; } = DiceGroup.Players;
		public string AdditionalDiceOnHitMessage { get; set; }
		public string FailMessage { get; set; }
		public string OnThrowSound { get; set; }
		public double HiddenThreshold { get; set; }
		public bool IsMagic { get; set; }
		public bool SuppressLegacyRoll { get; set; }

		// TODO: Remove Kind...
		public VantageKind VantageKind { get; set; }
		
		public int MinCrit { get; set; }
		public double Modifier { get; set; }
		public List<CardModifier> CardModifiers { get; set; } = new List<CardModifier>();
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

		public DiceRoll(DiceRollType diceRollType, VantageKind kind = VantageKind.Normal, string damageDice = "")
		{
			Type = diceRollType;
			DamageHealthExtraDice = damageDice;
			VantageKind = kind;
			ThrowPower = 1.0;
			MinCrit = 20;  // Some feats allow for a 19 to crit.
										 //SuccessMessage = "Success!";
										 //FailMessage = "Fail!";
										 //CritFailMessage = "Critical Fail!";
										 //CritSuccessMessage = "Critical Success!";
			MinDamage = 0;
			CritFailMessage = "";
			CritSuccessMessage = "";
			SuccessMessage = "";
			FailMessage = "";
			SkillCheck = Skills.none;
			SavingThrow = Ability.none;
			SpellName = "";
			// TODO: Set DiceGroup to the appropriate group.
			//DiceGroup = DiceGroup.Viewers;
		}

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

		// TODO: Put DiceRollType as a property on actionShortcut
		public static DiceRoll GetFrom(PlayerActionShortcut actionShortcut, Character player = null)
		{
			DieRollDetails bonusDie = DieRollDetails.From("");
			if (actionShortcut.CarriedWeapon != null)
			{
				if (actionShortcut.CarriedWeapon.Weapon.RequiresAmmunition() && player != null && player.ReadiedAmmunition != null)
					if (!string.IsNullOrEmpty(player.ReadiedAmmunition.DamageBonusStr))
						bonusDie.AddRoll(player.ReadiedAmmunition.DamageBonusStr);
			}
			
			DiceRoll diceRoll = new DiceRoll(actionShortcut.Type);
			if (player != null)
				diceRoll.SingleOwnerId = player.playerID;
			diceRoll.AdditionalDiceOnHit = actionShortcut.AddDiceOnHit;
			diceRoll.AdditionalDiceOnHitMessage = actionShortcut.AddDiceOnHitMessage;
			diceRoll.AddCritFailMessages(actionShortcut.Type);
			string dieToRoll;
			if (actionShortcut.HasInstantDice())
				dieToRoll = actionShortcut.InstantDice;
			else
				dieToRoll = actionShortcut.Dice;

			if (bonusDie.DieCount > 0)
			{
				DieRollDetails tempDiceRoll = DieRollDetails.From(dieToRoll);
				tempDiceRoll.AddDetails(bonusDie);
				diceRoll.DamageHealthExtraDice = tempDiceRoll.ToString();
			}
			else
				diceRoll.DamageHealthExtraDice = dieToRoll;

			string overrideReplaceDamageDice = null;
			if (player != null)
				overrideReplaceDamageDice = player.overrideReplaceDamageDice;
			if (!string.IsNullOrEmpty(overrideReplaceDamageDice))
			{
				DieRollDetails originalRoll = DieRollDetails.From(diceRoll.DamageHealthExtraDice);
				DieRollDetails replacementRoll = DieRollDetails.From(overrideReplaceDamageDice);
				if (replacementRoll.FirstDescriptor == "()")
					replacementRoll.FirstDescriptor = originalRoll.FirstDescriptor;

				if (replacementRoll.FirstOffset == 0)
					replacementRoll.FirstOffset = originalRoll.FirstOffset;

				overrideReplaceDamageDice = replacementRoll.ToString();
				diceRoll.DamageHealthExtraDice = overrideReplaceDamageDice;
			}

			diceRoll.DamageType = DamageType.None;  // Consider deprecating.
			diceRoll.IsMagic = actionShortcut.UsesMagic || actionShortcut.Type == DiceRollType.WildMagicD20Check || player.usesMagicThisRoll || player.usesMagicAmmunitionThisRoll;
			diceRoll.MinDamage = actionShortcut.MinDamage;
			diceRoll.Modifier = actionShortcut.ToHitModifier;
			diceRoll.SecondRollTitle = actionShortcut.AdditionalRollTitle;
			if (actionShortcut.Spell != null)
			{
				diceRoll.Modifier = player.SpellAttackBonus;
				diceRoll.SpellName = actionShortcut.Spell.Name;
				if (actionShortcut.Spell.MustRollDiceToCast())
					diceRoll.DamageHealthExtraDice = actionShortcut.Spell.DieStr;
			}

			return diceRoll;
		}

		public void AddCritFailMessages(DiceRollType type)
		{
			switch (type)
			{
				// TODO: Make this data-driven:
				case DiceRollType.SkillCheck:
					CritFailMessage = "COMPLETE FAILURE!";
					CritSuccessMessage = "Nat 20!";
					SuccessMessage = "Success!";
					FailMessage = "Fail!";
					break;
				case DiceRollType.Attack:
				case DiceRollType.ChaosBolt:
					CritFailMessage = "SPECTACULAR MISS!";
					CritSuccessMessage = "Critical Hit!";
					SuccessMessage = "Hit!";
					FailMessage = "Miss!";
					break;
				case DiceRollType.SavingThrow:
				case DiceRollType.OnlyTargetsSavingThrow:
				case DiceRollType.DamagePlusSavingThrow:
					CritFailMessage = "COMPLETE FAILURE!";
					CritSuccessMessage = "Critical Success!";
					SuccessMessage = "Success!";
					FailMessage = "Fail!";
					break;
				case DiceRollType.DeathSavingThrow:
					CritFailMessage = "COMPLETE FAILURE!";
					CritSuccessMessage = "Critical Success!";
					SuccessMessage = "Success!";
					FailMessage = "Fail!";
					break;
			}
		}
		public void SetHiddenThreshold(string fromText = "10")
		{
			if (Type == DiceRollType.DeathSavingThrow)
				HiddenThreshold = 10;
			else if (Type == DiceRollType.Initiative || Type == DiceRollType.NonCombatInitiative)
				HiddenThreshold = -100;
			else if (Type == DiceRollType.DamagePlusSavingThrow || Type == DiceRollType.OnlyTargetsSavingThrow)
				return;  // HiddenThreshold will be set elsewhere.
			else if (Type == DiceRollType.DamageOnly || Type == DiceRollType.HealthOnly || Type == DiceRollType.ExtraOnly)
				HiddenThreshold = 0;
			else if (Type == DiceRollType.ViewerRoll)
				HiddenThreshold = 0;
			else if (double.TryParse(fromText, out double thresholdResult))
				HiddenThreshold = thresholdResult;
		}
		public List<int> GetCreatureIds()
		{
			List<int> result = new List<int>();

			if (SingleOwnerId != int.MinValue)
				result.Add(SingleOwnerId);
			foreach (DiceDto diceDto in DiceDtos)
				if (!result.Contains(diceDto.CreatureId))
					result.Add(diceDto.CreatureId);

			foreach (PlayerRollOptions playerRollOptions in PlayerRollOptions)
				if (!result.Contains(playerRollOptions.PlayerID))
					result.Add(playerRollOptions.PlayerID);
			return result;
			
		}

		public bool HasDamage(DamageType damage)
		{
			if (DamageHealthExtraDice != null)
			{
				DieRollDetails dieRollDetails = DieRollDetails.From(DamageHealthExtraDice);
				foreach (Roll roll in dieRollDetails.Rolls)
					if (DndUtils.ToDamage(roll.Descriptor) == damage)
						return true;
			}

			if (DiceDtos?.Any() == true)
				foreach (DiceDto diceDto in DiceDtos)
					if (diceDto.DamageType == damage)
						return true;

			return false;
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
