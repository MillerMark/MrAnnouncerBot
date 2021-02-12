using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Dispels this Magic item. Valid when called inside any Magic event code.")]
	public class DispelMagic : DndFunction
	{
		public static event DispelMagicEventHandler RequestDispelMagic;
		public override string Name { get; set; } = "DispelMagic";

		public static void OnRequestDispelMagic(DispelMagicEventArgs ea)
		{
			RequestDispelMagic?.Invoke(ea);
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0);

			CreaturePlusModId recipient = Expressions.GetCustomData<CreaturePlusModId>(evaluator.Variables);
			if (recipient == null)
				return null;
			OnRequestDispelMagic(new DispelMagicEventArgs(recipient));
			return null;
		}
	}
}
