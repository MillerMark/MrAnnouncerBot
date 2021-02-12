using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{


	[Tooltip("Reveals the secret card associated with the magic (only valid on DnD Magic spreadsheet).")]
	public class RevealCard : DndFunction
	{
		public static event CreaturePlusModIdEventHandler RequestCardReveal;
		public override string Name { get; set; } = "RevealCard";
		
		public static void OnRequestCardReveal(CreaturePlusModIdEventArgs ea)
		{
			RequestCardReveal?.Invoke(ea);
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0);

			CreaturePlusModId recipient = Expressions.GetCustomData<CreaturePlusModId>(evaluator.Variables);
			OnRequestCardReveal(new CreaturePlusModIdEventArgs(recipient));
			return null;
		}
	}
}
