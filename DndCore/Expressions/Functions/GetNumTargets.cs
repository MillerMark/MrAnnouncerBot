using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{

	[Tooltip("Returns the number of targets of the specified type.")]
	[Param(1, typeof(TargetStatus), "targetStatus", "One of Friendly, Adversarial, Unknown, or AllTargets.", ParameterIs.Required)]
	public class GetNumTargets : DndFunction
	{
		public static event TargetCountEventHandler RequestTargetCount;

		public static void OnRequestTargetCount(object sender, TargetCountEventArgs ea)
		{
			RequestTargetCount?.Invoke(sender, ea);
		}

		public override string Name { get; set; } = "GetNumTargets";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);
			TargetStatus targetStatus = DndUtils.GetTargetStatus(args[0]);
			TargetCountEventArgs ea = new TargetCountEventArgs();
			ea.TargetStatus = targetStatus;
			OnRequestTargetCount(player, ea);
			return ea.Count;
		}
	}
}
