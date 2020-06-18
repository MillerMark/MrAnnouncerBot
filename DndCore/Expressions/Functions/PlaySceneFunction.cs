using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Plays the specified scene in OBS.")]
	[Param(1, typeof(string), "sceneName", "The name of scene in OBS.", ParameterIs.Required)]
	public class PlaySceneFunction : DndFunction
	{
		public delegate void PlaySceneEventHandler(object sender, PlaySceneEventArgs ea);
		public static event PlaySceneEventHandler RequestPlayScene;
		public static void OnRequestPlayScene(string sceneName)
		{
			PlaySceneEventArgs ea = new PlaySceneEventArgs(sceneName);
			RequestPlayScene?.Invoke(null, ea);
		}
		public override string Name => "PlayScene";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			string sceneName = evaluator.Evaluate<string>(args[0]);

			OnRequestPlayScene(sceneName);

			return null;
		}
	}
}
