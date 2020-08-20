using System;
using System.Diagnostics;

namespace DndCore
{
	[DebuggerDisplay("{SpellCaster?.firstName,nq}: {Spell?.Name,nq}")]
	public class CastedSpell
	{
		public event EventHandler OnDispel;
		static CastedSpell()
		{
			Validation.ValidationFailing += Validation_Failing;
		}

		public CastedSpell(Spell spell, Character spellCaster)
		{
			Target = spellCaster.ActiveTarget;
			SpellCaster = spellCaster;
			Spell = spell;
		}
		public Spell Spell { get; set; }
		public Character SpellCaster { get; set; }
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

		public void Dispel(Character player)
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

		public bool ValidationHasFailed(Character spellCaster, Target target)
		{
			validationFailed = false;
			Spell.TriggerValidate(spellCaster, target, this);
			return validationFailed;
		}

		public bool HasSpellCasterConcentration()
		{
			if (SpellCaster?.concentratedSpell == null)
				return false;

			return SpellCaster.concentratedSpell.Spell.Name == Spell.Name;
		}
	}
}
