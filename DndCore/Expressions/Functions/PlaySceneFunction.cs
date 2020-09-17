using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Plays the specified scene in OBS.")]
	[Param(1, typeof(string), "sceneName", "The name of scene in OBS.", ParameterIs.Required)]
	[Param(2, typeof(double), "delaySec", "An optional time to wait before playing the scene, in seconds.", ParameterIs.Optional)]
	[Param(3, typeof(double), "durationSec", "An optional time to wait before returning to a foundation scene, in seconds.", ParameterIs.Optional)]
	public class PlaySceneFunction : DndFunction
	{
		public delegate void PlaySceneEventHandler(object sender, PlaySceneEventArgs ea);
		public static event PlaySceneEventHandler RequestPlayScene;
		public static void OnRequestPlayScene(string sceneName, double delayMs, double returnSec)
		{
			PlaySceneEventArgs ea = new PlaySceneEventArgs(sceneName, delayMs, returnSec);
			RequestPlayScene?.Invoke(null, ea);
		}
		public override string Name => "PlayScene";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1, 3);

			string sceneName = evaluator.Evaluate<string>(args[0]);
			double delaySec = 0;
			double returnSec = 0;
			if (args.Count > 1)
			{
				delaySec = Expressions.GetDouble(args[1]);
				if (args.Count > 2)
					returnSec = Expressions.GetDouble(args[2]);
			}

			OnRequestPlayScene(sceneName, delaySec, returnSec);

			return null;
		}
	}
}
