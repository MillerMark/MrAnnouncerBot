using System;

namespace DndCore
{
	public class SpellChangedEventArgs : EventArgs
	{
		public SpellChangedEventArgs(Creature creature, string spellName, SpellState spellState)
		{
			Creature = creature;
			SpellName = spellName;
			SpellState = spellState;
		}
		public SpellState SpellState { get; set; }
		public string SpellName { get; set; }
		public Creature Creature { get; set; }
	}
}
