using System;

namespace DndCore
{
	public class CastedSpell
	{
		public CastedSpell(Spell spell, Character spellCaster)
		{
			TimeSpellWasCast = DndTimeClock.Instance.Time;
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

		public DateTime TimeSpellWasCast { get; set; }
		public string DieStr { get => Spell.DieStr; set => Spell.DieStr = value; }
		public string DieStrRaw { get => Spell.DieStrRaw; set { } }
		public int SpellSlotLevel { get => Spell.SpellSlotLevel; set => Spell.SpellSlotLevel = value; }
		public int Level { get => Spell.Level; set => Spell.Level = value; }
		public bool Active { get; set; }

		public void Casting()
		{
			SpellCaster.CheckConcentration(this);
			SpellCaster.ShowPlayerCasting(this);
			Spell.TriggerCasting(SpellCaster, Target, this);
		}

		public void Prepare()
		{
			//SpellCaster.CheckConcentration(this);
			SpellCaster.PrepareSpell(this);
		}

		public void CastingWithItem()
		{
			SpellCaster.ShowPlayerCasting(this);
			Spell.TriggerCasting(SpellCaster, Target, this);
		}

		public void Cast()
		{
			Active = true;
			Spell.TriggerCast(SpellCaster, Target, this);
		}

		public void Dispel()
		{
			if (!Active)
				return;
			Active = false;
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
	}
}
