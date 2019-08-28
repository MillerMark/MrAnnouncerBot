using System.Collections.Generic;

namespace DndCore
{
	public class CastedSpell
	{
		public Spell Spell { get; set; }
		public SpellTarget Target { get; set; }
		public List<WindupDto> Windups = new List<WindupDto>();

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
