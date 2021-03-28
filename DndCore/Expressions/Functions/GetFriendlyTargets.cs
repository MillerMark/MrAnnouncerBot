using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Gets friendly targets, and assigns them to the system variable FriendlyTargets, and also returning those targets.")]
	[Param(1, typeof(int), "maxTargets", "The maximum number of targets to acquire.", ParameterIs.Required)]
	public class GetFriendlyTargets : DndFunction
	{
		public static event TargetEventHandler RequestTarget;
		public override string Name { get; set; } = "GetFriendlyTargets";

		public static void OnRequestTarget(TargetEventArgs ea)
		{
			RequestTarget?.Invoke(ea);
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, RollResults dice = null)
		{
			ExpectingArguments(args, 1);
			TargetEventArgs ea = new TargetEventArgs();
			ea.SuggestedTargetType = TargetType.Friendly;
			ea.MaxTargets = Expressions.GetInt(args[0], player, target, spell);

			OnRequestTarget(ea);
			SystemVariables.FriendlyTargets = ea.Target;
			return ea.Target;
		}
	}
}
