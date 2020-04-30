using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class GetWeaponTarget : DndFunction
	{
		public override string Name { get; set; } = "GetWeaponTarget";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0, 1);

			string weaponFilter = null;
			if (args.Count > 0)
				weaponFilter = Expressions.GetStr(args[0], player, target, spell);
			if (player != null)
			{
				return player.ChooseWeapon(weaponFilter);
			}

			return null;
		}
	}
}
