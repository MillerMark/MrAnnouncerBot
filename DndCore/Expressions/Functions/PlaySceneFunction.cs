using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class PlaySceneEventArgs : EventArgs
	{
		
		public PlaySceneEventArgs(string sceneName)
		{
			SceneName = sceneName;
		}

		public string SceneName { get; set; }
	}
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

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target, CastedSpell spell)
		{
			ExpectingArguments(args, 1);

			string sceneName = evaluator.Evaluate<string>(args[0]);

			OnRequestPlayScene(sceneName);

			return null;
		}
	}
}
