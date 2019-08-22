using System;
using System.Linq;

namespace DndCore
{
	public class Spell
	{
		public int Range { get; set; }
		public DndTimeSpan Duration { get; set; }
		public DndTimeSpan CastingTime { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public SpellComponents Components { get; set; }
		public string Material { get; set; }
		public int OwnerId { get; set; }
		public bool RequiresConcentration { get; set; }

		public Spell()
		{

		}
	}
}
