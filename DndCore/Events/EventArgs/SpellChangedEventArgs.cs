using System;

namespace DndCore
{
	public class SpellChangedEventArgs : EventArgs
	{
		public SpellChangedEventArgs(Character player, string spellName, SpellState spellState)
		{
			Player = player;
			SpellName = spellName;
			SpellState = spellState;
		}
		public SpellState SpellState { get; set; }
		public string SpellName { get; set; }
		public Character Player { get; set; }
	}
}
