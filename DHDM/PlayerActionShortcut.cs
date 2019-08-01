using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public class PlayerActionShortcut
	{
		public List<WindupDto> Windups { get; }
		public string WindupName { get; set; }
		static int attackIndex;
		public int Modifier { get; set; }
		public string Dice { get; set; }
		public string Name { get; set; }
		public bool UsesMagic { get; set; }
		public int PlayerID { get; set; }
		public int DC { get; set; }
		public TurnPart Part { get; set; }

		public Ability Ability { get; set; }
		public DiceRollType Type { get; set; }
		public int Index { get; set; }
		public bool Healing { get; set; }
		public int LimitCount { get; set; }
		public DndTimeSpan LimitSpan { get; set; }
		public string Description { get; set; }
		public VantageKind VantageMod { get; set; }
		public string AddDice { get; set; }
		public string InstantDice { get; set; }
		public string AdditionalRollTitle { get; set; }
		public int MinDamage { get; set; }
		public PlayerActionShortcut()
		{
			MinDamage = 0;
			Ability = Ability.None;
			UsesMagic = false;
			Modifier = 0;
			Type = DiceRollType.None;
			Index = attackIndex;
			attackIndex++;
			Part = TurnPart.Action;
			LimitCount = 0;
			LimitSpan = DndTimeSpan.Never;
			Description = "";
			AddDice = "";
			InstantDice = "";
			AdditionalRollTitle = "";
			Dice = "";
			VantageMod = VantageKind.Normal;
			Windups = new List<WindupDto>();
		}

		public static void PrepareForCreation()
		{
			attackIndex = 0;
		}
	}
}
