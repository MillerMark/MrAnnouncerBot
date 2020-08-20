using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Selects a target for a spell (currently not implemented in DHDM).")]
	[Param(1, typeof(bool), "showXamlUI", "Shows the XAML Target Picker if no creatures are currently targeted.", ParameterIs.Optional)]
	// TODO: Add parameters to support filtering/setting the Target (like SpellTargetShape).
	public class SelectTargetFunction : DndFunction
	{
		public static event TargetEventHandler RequestSelectTarget;
		public static void OnRequestSelectTarget(TargetEventArgs ea)
		{
			RequestSelectTarget?.Invoke(ea);
		}
		public override string Name { get; set; } = "SelectTarget";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0, 1);
			bool showXamlUI = true;
			if (args.Count > 0)
				showXamlUI = Expressions.GetBool(args[0], player, target, spell, dice);
			TargetEventArgs ea = new TargetEventArgs();
			ea.Player = player;
			ea.Target = target;
			ea.ShowXamlUI = showXamlUI;
			OnRequestSelectTarget(ea);

			return ea.Target;
		}
	}
}
