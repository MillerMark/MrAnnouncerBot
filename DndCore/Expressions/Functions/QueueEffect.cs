using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Queues up a CardEvent to be played in the D&D game.")]
	[Param(1, typeof(string), "cardEventName", "The category of the card event to queue.", ParameterIs.Required)]
	[Param(2, typeof(string), "cardUserName", "The name of the user who played this card.", ParameterIs.Required)]

	public class QueueEffect : DndFunction
	{
		public static event QueueEffectEventHandler RequestCardEventQueuing;
		public override string Name { get; set; } = "QueueEffect";

		protected static void OnRequestCardEventQueuing(QueueEffectEventArgs ea)
		{
			RequestCardEventQueuing?.Invoke(null, ea);
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, RollResults dice = null)
		{
			ExpectingArguments(args, 2, 10);
			string cardEventName = Expressions.GetStr(args[0]);
			string cardUserName = Expressions.GetStr(args[1]);

			object data1 = null;
			object data2 = null;
			object data3 = null;
			object data4 = null;
			object data5 = null;
			object data6 = null;
			object data7 = null;
			object data8 = null;

			if (args.Count > 2)
				data1 = GiveMagicFunction.GetData(args[2], player, target, spell);
			if (args.Count > 3)
				data2 = GiveMagicFunction.GetData(args[3], player, target, spell);
			if (args.Count > 4)
				data3 = GiveMagicFunction.GetData(args[4], player, target, spell);
			if (args.Count > 5)
				data4 = GiveMagicFunction.GetData(args[5], player, target, spell);
			if (args.Count > 6)
				data5 = GiveMagicFunction.GetData(args[6], player, target, spell);
			if (args.Count > 7)
				data6 = GiveMagicFunction.GetData(args[7], player, target, spell);
			if (args.Count > 8)
				data7 = GiveMagicFunction.GetData(args[8], player, target, spell);
			if (args.Count > 9)
				data8 = GiveMagicFunction.GetData(args[9], player, target, spell);

			OnRequestCardEventQueuing(new QueueEffectEventArgs(cardEventName, cardUserName, data1, data2, data3, data4, data5, data6, data7, data8));
			return null;
		}
	}
}
