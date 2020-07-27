using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Plays the specified scene in OBS.")]
	[Param(1, typeof(string), "sceneName", "The name of scene in OBS.", ParameterIs.Required)]
	[Param(2, typeof(int), "delayMs", "An optional time to wait before playing the scene, in milliseconds.", ParameterIs.Optional)]
	public class PlaySceneFunction : DndFunction
	{
		public delegate void PlaySceneEventHandler(object sender, PlaySceneEventArgs ea);
		public static event PlaySceneEventHandler RequestPlayScene;
		public static void OnRequestPlayScene(string sceneName, int delayMs)
		{
			PlaySceneEventArgs ea = new PlaySceneEventArgs(sceneName, delayMs);
			RequestPlayScene?.Invoke(null, ea);
		}
		public override string Name => "PlayScene";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1, 2);

			string sceneName = evaluator.Evaluate<string>(args[0]);
			int delayMs = 0;
			if (args.Count > 1)
				delayMs = Expressions.GetInt(args[1]);

			OnRequestPlayScene(sceneName, delayMs);

			return null;
		}
	}
}
