namespace DndCore
{
	public class CastedSpell
	{
		public Spell Spell { get; set; }
		public SpellTarget Target { get; set; }
		public CastedSpell(Spell spell, SpellTarget target)
		{
			Spell = spell;
			Target = target;
		}
		public CastedSpell()
		{

		}
	}
}
