using System;

namespace DndCore
{
	public class CastedSpell
	{
		public CastedSpell(Spell spell, Character spellCaster, object target = null)
		{
			TimeSpellWasCast = DndTimeClock.Instance.Time;
			Target = target;
			SpellCaster = spellCaster;
			Spell = spell;
		}
		public Spell Spell { get; set; }
		public Character SpellCaster { get; set; }
		public object Target { get; set; }

		public Creature TargetCreature
		{
			get
			{
				return Target as Creature;
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
	}
}
