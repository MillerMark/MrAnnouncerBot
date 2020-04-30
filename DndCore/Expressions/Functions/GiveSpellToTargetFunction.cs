using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class GiveSpellToTargetFunction : DndFunction
	{
		public override string Name { get; set; } = "GiveSpellToTarget";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			if (target == null)
				return null;
			ExpectingArguments(args, 1, 8);  // up to seven optional parameters of any data type.

			string spellName = Expressions.GetStr(args[0], player, target, spell);
			object data1 = null;
			object data2 = null;
			object data3 = null;
			object data4 = null;
			object data5 = null;
			object data6 = null;
			object data7 = null;
			if (args.Count > 1)
				data1 = Expressions.Get(args[1], player, target, spell);
			if (args.Count > 2)
				data2 = Expressions.Get(args[2], player, target, spell);
			if (args.Count > 3)
				data3 = Expressions.Get(args[3], player, target, spell);
			if (args.Count > 4)
				data4 = Expressions.Get(args[4], player, target, spell);
			if (args.Count > 5)
				data5 = Expressions.Get(args[5], player, target, spell);
			if (args.Count > 6)
				data6 = Expressions.Get(args[6], player, target, spell);
			if (args.Count > 7)
				data7 = Expressions.Get(args[7], player, target, spell);

			if (target == null || player.Game == null)
				return null;

			foreach (int playerId in target.PlayerIds)
			{
				Character recipient = player.Game.GetPlayerFromId(playerId);
				if (recipient == null)
					break;
				KnownSpell knownSpell = new KnownSpell();
				knownSpell.SpellName = spellName;
				knownSpell.Player = recipient;
				recipient.GiveSpell(knownSpell, data1, data2, data3, data4, data5, data6, data7);
			}

			return null;
		}
	}
}
