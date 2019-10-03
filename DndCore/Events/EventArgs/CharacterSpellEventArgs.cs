using System;

namespace DndCore
{
	public class CharacterSpellEventArgs : SpellEventArgs
	{

		public CharacterSpellEventArgs(Character character, Spell spell) : base(spell)
		{
			Character = character;
		}

		public Character Character { get; private set; }
	}
}

