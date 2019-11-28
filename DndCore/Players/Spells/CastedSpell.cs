using System;

namespace DndCore
{
	public class CastedSpell
	{
		public CastedSpell(Spell spell, Character spellCaster, Creature targetCreature = null)
		{
			TimeSpellWasCast = DndTimeClock.Instance.Time;
			TargetCreature = targetCreature;
			SpellCaster = spellCaster;
			Spell = spell;
		}
		public Spell Spell { get; set; }
		public Character SpellCaster { get; set; }
		public Creature TargetCreature { get; set; }
		public DateTime TimeSpellWasCast { get; set; }
		public string DieStr { get => Spell.DieStr; set => Spell.DieStr = value; }
		public string DieStrRaw { get => Spell.DieStrRaw; set { } }
		public int SpellSlotLevel { get => Spell.SpellSlotLevel; set => Spell.SpellSlotLevel = value; }
		public int Level { get => Spell.Level; set => Spell.Level = value; }

		public void Casting()
		{
			SpellCaster.CheckConcentration(this);
			SpellCaster.ShowPlayerCasting(this);
			Spell.TriggerCasting(SpellCaster, TargetCreature, this);
		}

		public void CastingWithItem()
		{
			SpellCaster.ShowPlayerCasting(this);
			Spell.TriggerCasting(SpellCaster, TargetCreature, this);
		}

		public void Cast()
		{
			Spell.TriggerCast(SpellCaster, TargetCreature, this);
		}

		public void Dispel()
		{
			Spell.TriggerDispel(SpellCaster, TargetCreature, this);
		}

		public void Dispel(Character player)
		{
			player.Dispel(this);
		}
	}
}
