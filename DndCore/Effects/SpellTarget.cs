using System;
using System.Linq;

namespace DndCore
{
	public class SpellTarget
	{
		public SpellTargetType Target { get; set; }
		public int PlayerId { get; set; }
		public Vector Location { get; set; }
		public int Range { get; set; }
		public SpellTarget()
		{

		}
	}
}
