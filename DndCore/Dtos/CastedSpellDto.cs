using System.Collections.Generic;

namespace DndCore
{
	public class CastedSpellDto
	{
		public Spell Spell { get; set; }
		public SpellTarget Target { get; set; }
		public List<WindupDto> Windups = new List<WindupDto>();

		public CastedSpellDto(Spell spell, SpellTarget target)
		{
			Spell = spell;
			Target = target;
		}
		public CastedSpellDto()
		{

		}
	}
}
