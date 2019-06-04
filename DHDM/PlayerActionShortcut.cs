using System;
using System.Linq;
using DndCore;

namespace DHDM
{
	public class PlayerActionShortcut
	{
		static int attackIndex;
		public int Modifier { get; set; }
		public string Dice { get; set; }
		public string Name { get; set; }
		public bool UsesMagic { get; set; }
		public int PlayerID { get; set; }
		public int DC { get; set; }
		public Ability Ability { get; set; }
		public DiceRollType Type { get; set; }
		public int Index { get; set; }
		public bool Healing { get; set; }
		public PlayerActionShortcut()
		{
			Ability = Ability.None;
			UsesMagic = false;
			Modifier = 0;
			Type = DiceRollType.None;
			Index = attackIndex;
			attackIndex++;
		}
		public static void PrepareForCreation()
		{
			attackIndex = 0;
		}
	}
}
