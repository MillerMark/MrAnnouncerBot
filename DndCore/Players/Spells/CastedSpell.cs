using System;
using System.Diagnostics;

namespace DndCore
{
	[DebuggerDisplay("{SpellCaster?.firstName,nq}: {Spell?.Name,nq}")]
	public class CastedSpell
	{
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
			Spell.TriggerCast(SpellCaster, Target, this);
		}

		protected virtual void OnOnDispel(object sender, EventArgs e)
		{
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
			ValidationResult validationResult = new ValidationResult();
			if (Spell.MinTargetsToCast > 0)
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
				if (target.Count < Spell.MinTargetsToCast)
				{
					validationResult.ValidationAction = ValidationAction.Stop;
					if (Spell.MinTargetsToCast == 1)
						validationResult.MessageOverPlayer = $"Need at least one target to cast {Spell.Name}!";
					else
						validationResult.MessageOverPlayer = $"Need at least {Spell.MinTargetsToCast} targets to cast {Spell.Name}!";
				}
				if (target.Count > Spell.MaxTargetsToCast)
				{
					validationResult.ValidationAction = ValidationAction.Stop;
					if (Spell.MaxTargetsToCast == 1)
						validationResult.MessageOverPlayer = $"Only one target is allowed for {Spell.Name}!";
					else
						validationResult.MessageOverPlayer = $"Only {Spell.MaxTargetsToCast} targets are allowed for {Spell.Name}!";
				}
			}
			TriggerSpellValidation(spellCaster, target, validationResult);

			return validationResult;
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
