using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Selects a weapon from the active player that matches the specified filter.")]
	[Param(1, typeof(string), "weaponFilter", "A semicolon-separated list of valid weapon types (e.g., \"Club;Quarterstaff\").", ParameterIs.Optional)]
	public class GetWeaponTarget : DndFunction
	{
		public override string Name { get; set; } = "GetWeaponTarget";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, RollResults dice = null)
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
