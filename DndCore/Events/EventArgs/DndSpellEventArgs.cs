using System;
using System.Linq;

namespace DndCore
{
	public class CastedSpellEventArgs : DndGameEventArgs
	{
		public CastedSpell CastedSpell { get; private set; }

		public CastedSpellEventArgs(DndGame game, CastedSpell castedSpell): base(game)
		{
			CastedSpell = castedSpell;
		}

		public CastedSpellEventArgs()
		{

		}
	}
}
