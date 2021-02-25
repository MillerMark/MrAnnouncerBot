using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Gives the specified magic to the specified target with the specified parameters")]
	[Param(1, typeof(Target), "target", "The Target to receive the magic item. Use SelectTarget to get one interactively.", ParameterIs.Required)]
	[Param(2, typeof(string), "magicItem", "The name of the magic item to give.", ParameterIs.Required)]
	[Param(3, typeof(object), "data1", "Data parameter 1 for the magic item.", ParameterIs.Optional)]
	[Param(4, typeof(object), "data2", "Data parameter 2 for the magic item.", ParameterIs.Optional)]
	[Param(5, typeof(object), "data3", "Data parameter 3 for the magic item.", ParameterIs.Optional)]
	[Param(6, typeof(object), "data4", "Data parameter 4 for the magic item.", ParameterIs.Optional)]
	[Param(7, typeof(object), "data5", "Data parameter 5 for the magic item.", ParameterIs.Optional)]
	[Param(8, typeof(object), "data6", "Data parameter 6 for the magic item.", ParameterIs.Optional)]
	[Param(9, typeof(object), "data7", "Data parameter 7 for the magic item.", ParameterIs.Optional)]
	[Param(10, typeof(object), "data8", "Data parameter 8 for the magic item.", ParameterIs.Optional)]
	public class GiveMagicFunction : DndFunction
	{
		public override string Name { get; set; } = "GiveMagic";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 2, 10);
			if (player != null)
			{
				if (target == null)
					target = Expressions.Get<Target>(args[0]);

				string magicItemName = Expressions.GetStr(args[1]);

				object data1 = null;
				object data2 = null;
				object data3 = null;
				object data4 = null;
				object data5 = null;
				object data6 = null;
				object data7 = null;
				object data8 = null;

				if (args.Count > 2)
					data1 = GetData(args[2], player, target, spell);
				if (args.Count > 3)
					data2 = GetData(args[3], player, target, spell);
				if (args.Count > 4)
					data3 = GetData(args[4], player, target, spell);
				if (args.Count > 5)
					data4 = GetData(args[5], player, target, spell);
				if (args.Count > 6)
					data5 = GetData(args[6], player, target, spell);
				if (args.Count > 7)
					data6 = GetData(args[7], player, target, spell);
				if (args.Count > 8)
					data7 = GetData(args[8], player, target, spell);
				if (args.Count > 9)
					data8 = GetData(args[9], player, target, spell);

				//if (target == null)
				//	target = player.ActiveTarget;
				// TODO: consider passing in the spell name as well for unique id (if castedSpell is valid).
				player.GiveMagic(magicItemName, spell, target, data1, data2, data3, data4, data5, data6, data7, data8);
			}

			return null;
		}

		public static object GetData(string arg, Creature player, Target target, CastedSpell spell)
		{
			arg = arg.Trim();
			if (arg.StartsWith("\""))
				return arg;
			return Expressions.Get(arg, player, target, spell);
		}
	}
}
