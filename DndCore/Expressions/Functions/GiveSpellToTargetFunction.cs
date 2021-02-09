using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Gives a the specified spell to the target with up to seven optional parameters that will be passed with that spell.")]
	[Param(1, typeof(string), "spellName", "The name of the spell.", ParameterIs.Required)]
	[Param(2, typeof(object), "data1", "Optional data parameter 1. Can be accessed by referencing \"data1\" from expressions for the given spell.", ParameterIs.Optional)]
	[Param(3, typeof(object), "data2", "Optional data parameter 2. Can be accessed by referencing \"data2\" from expressions for the given spell.", ParameterIs.Optional)]
	[Param(4, typeof(object), "data3", "Optional data parameter 3. Can be accessed by referencing \"data3\" from expressions for the given spell.", ParameterIs.Optional)]
	[Param(5, typeof(object), "data4", "Optional data parameter 4. Can be accessed by referencing \"data4\" from expressions for the given spell.", ParameterIs.Optional)]
	[Param(6, typeof(object), "data5", "Optional data parameter 5. Can be accessed by referencing \"data5\" from expressions for the given spell.", ParameterIs.Optional)]
	[Param(7, typeof(object), "data6", "Optional data parameter 6. Can be accessed by referencing \"data6\" from expressions for the given spell.", ParameterIs.Optional)]
	[Param(8, typeof(object), "data7", "Optional data parameter 7. Can be accessed by referencing \"data7\" from expressions for the given spell.", ParameterIs.Optional)]
	public class GiveSpellToTargetFunction : DndFunction
	{
		public override string Name { get; set; } = "GiveSpellToTarget";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
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
