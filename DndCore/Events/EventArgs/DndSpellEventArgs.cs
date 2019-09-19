using System;
using System.Linq;

namespace DndCore
{
	public class DndSpellEventArgs : DndGameEventArgs
	{
		public CastedSpell CastedSpell { get; private set; }

		public DndSpellEventArgs(DndGame game, CastedSpell castedSpell): base(game)
		{
			CastedSpell = castedSpell;
		}

		public DndSpellEventArgs()
		{

		}
	}
}
