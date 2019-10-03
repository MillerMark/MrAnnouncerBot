using System;

namespace DndCore
{
	public class SpellEventArgs : EventArgs
	{

		public SpellEventArgs(Spell spell)
		{
			Spell = spell;
		}

		public Spell Spell { get; private set; }
	}
}

