using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Animates the size of the live video feed for the active player.")]
	[Param(1, typeof(double), "percentOfNormalSize", "The percent of the normal video size to animate to.", ParameterIs.Required)]
	[Param(2, typeof(double), "timeMs", "The number of milliseconds for the animation to last.", ParameterIs.Optional)]
	public class AnimateLiveFeed : DndFunction
	{
		public static event LiveFeedEventHandler RequestLiveFeedResize;
		public override string Name { get; set; } = "AnimateLiveFeed";
		
		public static void OnRequestLiveFeedResize(object sender, LiveFeedEventArgs ea)
		{
			RequestLiveFeedResize?.Invoke(sender, ea);
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, RollResults dice = null)
		{
			ExpectingArguments(args, 2);
			if (player != null)
			{
				double targetScale = Expressions.GetDouble(args[0], player, target, spell) / 100.0;
				double timeMs = Expressions.GetDouble(args[1], player, target, spell);
				OnRequestLiveFeedResize(null, new LiveFeedEventArgs(player, targetScale, timeMs));
			}

			return null;
		}
	}
}
