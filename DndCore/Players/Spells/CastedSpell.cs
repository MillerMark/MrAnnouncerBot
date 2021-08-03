using System;
using System.Diagnostics;

namespace DndCore
{
	[DebuggerDisplay("{SpellCaster?.firstName,nq}: {Spell?.Name,nq}")]
	public class CastedSpell
	{
		static System.Collections.Generic.Dictionary<string, CastedSpell> activeSpells;
		public string ID { get; set; }
		public event EventHandler OnDispel;
		static CastedSpell()
		{
			Validation.ValidationFailing += Validation_Failing;
		}

		public CastedSpell(Spell spell, Creature spellCaster)
		{
			ID = Guid.NewGuid().ToString();
			Target = spellCaster.ActiveTarget;
			SpellCaster = spellCaster;
			Spell = spell;
		}
		public Spell Spell { get; set; }
		public Creature SpellCaster { get; set; }
		public Target Target { get; set; }
		public CarriedWeapon TargetWeapon
		{
			get
			{
				return Target?.Weapon;
			}
		}

		public static bool operator ==(CastedSpell left, CastedSpell right)
		{
			if ((object)left == null)
				return (object)right == null;
			else
				return left.Equals(right);
		}

		public static bool operator !=(CastedSpell left, CastedSpell right)
		{
			return !(left == right);
		}

		public override int GetHashCode()
		{
			// TODO: Modify this hash code calculation, if desired.
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is CastedSpell)
				return Equals((CastedSpell)obj);
			else if (obj is CastedSpell)
				return Equals((CastedSpell)obj);
			else
				return base.Equals(obj);
		}

		public bool Equals(CastedSpell castedSpell)
		{
			if (castedSpell == null)
				return false;
			return castedSpell.SpellCaster == SpellCaster && castedSpell.Spell.Name == Spell.Name;
		}

		public string DieStr { get => Spell.DieStr; set => Spell.DieStr = value; }
		public string DieStrRaw { get => Spell.DieStrRaw; set { } }
		public int SpellSlotLevel { get => Spell.SpellSlotLevel; set => Spell.SpellSlotLevel = value; }
		public int Level { get => Spell.Level; set => Spell.Level = value; }
		public bool Active { get; set; }
		public DateTime CastingTime { get; set; }
		public int CastingRound { get; set; }
		public int CastingTurnIndex { get; set; }

		public static System.Collections.Generic.Dictionary<string, CastedSpell> ActiveSpells { 
			get 
			{
				if (activeSpells == null)
					activeSpells = new System.Collections.Generic.Dictionary<string, CastedSpell>();
				return activeSpells; 
			}
			private set => activeSpells = value; 
		}

		public void PreparationComplete()
		{
			SpellCaster.ShowPlayerCasting(this);
			Spell.TriggerPreparationComplete(SpellCaster, Target, this);
		}

		public void PreparationStarted(DndGame game)
		{
			CastingTime = game.Clock.Time;
			if (game.InCombat)
			{
				CastingRound = game.RoundNumber;
				CastingTurnIndex = game.InitiativeIndex;
			}
			else
			{
				CastingRound = -1;
				CastingTurnIndex = -1;
			}
			SpellCaster.CheckConcentration(this);
			Spell.TriggerPreparing(SpellCaster, Target, this);
		}

		public void Prepare()
		{
			//SpellCaster.CheckConcentration(this);
			SpellCaster.PrepareSpell(this);
		}

		public void CastingWithItem()
		{
			SpellCaster.ShowPlayerCasting(this);
			Spell.TriggerPreparationComplete(SpellCaster, Target, this);
		}

		public void Cast()
		{
			Active = true;
			if (!ActiveSpells.ContainsKey(ID))
				ActiveSpells.Add(ID, this);
			Spell.TriggerCast(SpellCaster, Target, this);
		}

		protected virtual void OnOnDispel(object sender, EventArgs e)
		{
			ActiveSpells.Remove(ID);
			OnDispel?.Invoke(sender, e);
		}

		public void Dispel()
		{
			if (!Active)
				return;
			Active = false;
			OnOnDispel(this, EventArgs.Empty);
			Spell.TriggerDispel(SpellCaster, Target, this);
		}

		public void Dispel(Creature player)
		{
			player.Dispel(this);
		}

		public string GetSpellRawDieStr(string filter = null)
		{
			return Spell.GetSpellRawDieStr(filter);
		}

		static bool validationFailed;
		static DateTime lastWarningTime;
		static string lastWarningMessage;
		private static void Validation_Failing(object sender, ValidationEventArgs ea)
		{
			if (ea.ValidationAction == ValidationAction.Warn)
			{
				if (DateTime.Now - lastWarningTime < TimeSpan.FromSeconds(10) && lastWarningMessage == ea.DungeonMasterMessage)
				{
					ea.OverrideWarning = true;
					return;
				}
				lastWarningMessage = ea.DungeonMasterMessage;
				lastWarningTime = DateTime.Now;
			}
			validationFailed = true;
		}

		public ValidationResult GetValidation(Character spellCaster, Target target)
		{
			if (Spell.TargetDetails.MaxCreatures > 0)
			{
				// TODO: Add validation to make sure the number of targets do not exceed the max.

			}

			ValidationResult validationResult = new ValidationResult();

			try
			{
				//if (Spell.TargetDetails.Kind != TargetKind.None)
				//{
				//	if (Spell.TargetDetails.Kind.HasFlag(TargetKind.Volume))
				//	{
				//		if (Spell.TargetDetails.MaxCreatures == 0)
				//		{
				//			// Hey, no player/npc targets allowed.
				//			if (target != null && target.Count > 0)
				//			{
				//				// chill
				//				//validationResult.MessageOverPlayer = "No creature targets allowed";
				//				//validationResult.MessageToDm = $"Must clear all targets before casting {spellCaster.Name}'s {Spell.Name}.";
				//				//validationResult.ValidationAction = ValidationAction.Stop;
				//				//return validationResult;
				//			}
				//		}
				//	}
				//}

				ValidateRangeVolume(spellCaster, validationResult);
				if (validationResult.ValidationAction != ValidationAction.None)
					return validationResult;

				ValidateTargetCount(target, validationResult);
				if (validationResult.ValidationAction != ValidationAction.None)
					return validationResult;

				ValidateTargetedCreaturesInRange(spellCaster, target, validationResult);
				if (validationResult.ValidationAction != ValidationAction.None)
					return validationResult;
			}
			finally
			{
				TriggerSpellValidation(spellCaster, target, validationResult);
			}
			return validationResult;
		}

		private void ValidateTargetedCreaturesInRange(Character spellCaster, Target target, ValidationResult validationResult)
		{
			if (Spell.Range > 0)
			{
				if (Targeting.ExpectedTargetDetails.Kind.HasFlag(TargetKind.Creatures))
				{
					foreach (Creature creature in target.Creatures)
					{
						double feetFromCaster = DndUtils.TilesToFeet(spellCaster.MapPosition.DistanceTo(creature.MapPosition));
						if (feetFromCaster > Spell.Range)
						{
							validationResult.ValidationAction = ValidationAction.Stop;
							validationResult.MessageOverPlayer = $"{creature.firstName} is {Math.Round(feetFromCaster, 2)}ft from {spellCaster.firstName}.";
							break;
						}
					}

					if (target.PlayerIds != null)
						foreach (int playerId in target.PlayerIds)
						{
							Character player = AllPlayers.GetFromId(playerId);
							double feetFromCaster = DndUtils.TilesToFeet(spellCaster.MapPosition.DistanceTo(player.MapPosition));
							if (feetFromCaster > Spell.Range)
							{
								validationResult.ValidationAction = ValidationAction.Stop;
								validationResult.MessageOverPlayer = $"{player.firstName} is {Math.Round(feetFromCaster, 2)}ft from {spellCaster.firstName}.";
								break;
							}
						}

					if (validationResult.ValidationAction == ValidationAction.Stop)
						validationResult.MessageToDm = $"{Spell.Name} targets must be within {Spell.Range}ft of {spellCaster.firstName}.";
				}
			}
		}

		private void ValidateTargetCount(Target target, ValidationResult validationResult)
		{
			int maxTargetsToCast = Spell.MaxTargetsToCast;
			int minTargetsToCast = Spell.MinTargetsToCast;
			// TODO: consider moving this logic directly into the properties MinTargetsToCast and MaxTargetsToCast
			if (maxTargetsToCast == -1)  // Use spell ammo count
				maxTargetsToCast = Spell.AmmoCount;
			if (minTargetsToCast == -1)  // Use spell ammo count
				minTargetsToCast = Spell.AmmoCount;

			if (minTargetsToCast > 0 || maxTargetsToCast > 0)
			{
				// TODO: Add warnings if min threshold satisfied but more targets are available.

				/* 
					var targetCount = GetCount(target);
					if (targetCount == 0)
						ValidationFailed($"Select up to {spell_AmmoCount} target(s)!", "Select targets!", Stop);
					else if (targetCount > spell_AmmoCount)
						ValidationFailed($"Max targets for Enhance Ability ({spell_AmmoCount}) exceeded! Select fewer targets or cast at a higher spell slot!", "Too many targets!", Stop);
					else if (targetCount < spell_AmmoCount)
						ValidationFailed($"More targets for Enhance Ability are available ({spell_AmmoCount} total). Are you sure you want to cast?", "Select more targets?", Warn); 

				 */
				if (target.Count < minTargetsToCast)
				{
					validationResult.ValidationAction = ValidationAction.Stop;
					if (minTargetsToCast == 1)
						validationResult.MessageOverPlayer = $"Need at least one target to cast {Spell.Name}!";
					else
						validationResult.MessageOverPlayer = $"Need at least {minTargetsToCast} targets to cast {Spell.Name}!";
				}
				else if (maxTargetsToCast > 0 && target.Count < maxTargetsToCast)
				{
					validationResult.ValidationAction = ValidationAction.Warn;
					if (maxTargetsToCast == 1)
						validationResult.MessageOverPlayer = $"Missing a target for {Spell.Name}?";
					else
						validationResult.MessageOverPlayer = $"Only {target.Count} of {maxTargetsToCast} targets with {Spell.Name}?!";
				}
				else if (target.Count > maxTargetsToCast)
				{
					validationResult.ValidationAction = ValidationAction.Stop;
					if (maxTargetsToCast == 1)
						validationResult.MessageOverPlayer = $"Only one target allowed for {Spell.Name}!";
					else
						validationResult.MessageOverPlayer = $"Only {maxTargetsToCast} targets are allowed for {Spell.Name}!";
				}
			}
		}

		private void ValidateRangeVolume(Character spellCaster, ValidationResult validationResult)
		{
			if (Spell.Range > 0)
			{
				if (Targeting.ExpectedTargetDetails.Kind.HasFlag(TargetKind.Volume) || Targeting.ExpectedTargetDetails.Kind.HasFlag(TargetKind.Location))
				{
					double feetFromCaster = DndUtils.TilesToFeet(spellCaster.MapPosition.DistanceTo(Targeting.TargetPoint));
					if (feetFromCaster > Spell.Range)
					{
						validationResult.ValidationAction = ValidationAction.Stop;
						validationResult.MessageOverPlayer = $"Target out of range - {Spell.Range}ft!";
					}
				}
			}
		}

		private void TriggerSpellValidation(Character spellCaster, Target target, ValidationResult validationResult)
		{
			validationFailed = false;
			Spell.TriggerValidate(spellCaster, target, this);
			if (validationFailed)
				validationResult.ValidationAction = ValidationAction.Stop;
		}

		public bool HasSpellCasterConcentration()
		{
			if (SpellCaster?.concentratedSpell == null)
				return false;

			return SpellCaster.concentratedSpell.Spell.Name == Spell.Name;
		}
	}
}
