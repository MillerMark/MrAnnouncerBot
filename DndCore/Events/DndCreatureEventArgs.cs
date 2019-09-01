using System;
using System.Linq;

namespace DndCore
{
	public class DndCreatureEventArgs : EventArgs
	{
		public Creature Creature { get; set; }
		public DndCreatureEventArgs(Creature creature)
		{
			Creature = creature;
		}
		public DndCreatureEventArgs()
		{

		}
	}
}
