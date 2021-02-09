using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Selects a target for a spell (currently not implemented in DHDM).")]
	[Param(1, typeof(TargetStatus), "targetStatus", "The kinds of targets we can select.", ParameterIs.Required)]
	[Param(2, typeof(int), "minTargets", "The fewest targets that can be selected.", ParameterIs.Optional)]
	[Param(3, typeof(int), "maxTargets", "The most targets that can be selected.", ParameterIs.Optional)]
	
	// TODO: Add parameters to support filtering/setting the Target (like SpellTargetShape).
	public class SelectTargetFunction : DndFunction
	{
		public static event TargetEventHandler RequestSelectTarget;

		public SelectTargetFunction()
		{
			IsAsync = true;
		}

		//public async Task OnRequestSelectTarget(TargetEventArgs ea)
		//{
		//	TargetEventHandler handler = RequestSelectTarget;

		//	if (handler == null)
		//		return;

		//	Delegate[] invocationList = handler.GetInvocationList();
		//	Task[] handlerTasks = new Task[invocationList.Length];

		//	for (int i = 0; i < invocationList.Length; i++)
		//	{
		//		handlerTasks[i] = ((TargetEventHandler)invocationList[i])(ea);
		//	}

		//	await Task.WhenAll(handlerTasks);
		//}

		public static void OnRequestSelectTarget(TargetEventArgs ea)
		{
			RequestSelectTarget?.Invoke(ea);
		}
		public override string Name { get; set; } = "SelectTarget";

		TargetEventArgs ea;
		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature creature, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1, 3);
			TargetStatus targetStatus = Expressions.Get<TargetStatus>(args[0], creature, target, spell, dice);
			ea = new TargetEventArgs();
			if (args.Count > 1)
			{
				ea.MinTargets = Expressions.GetInt(args[1], creature, target, spell, dice);
				if (args.Count > 2)
				{
					ea.MaxTargets = Expressions.GetInt(args[2], creature, target, spell, dice);
				}
			}
			
			ea.Player = creature as Character;
			ea.Target = target;
			
			ea.ShowUI = true; 
			ea.TargetStatus = targetStatus;
			OnRequestSelectTarget(ea);
			return ea.Target;
		}
	}
}
