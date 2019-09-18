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
		public void Casting()
		{
			Spell.TriggerOnCasting(SpellCaster, TargetCreature, this);
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
