using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Sets the specified OBS source visibility.")]
	[Param(1, typeof(string), "sourceName", "The name of the OBS source to change visibility.", ParameterIs.Required)]
	[Param(2, typeof(bool), "visible", "True for visible or false to hide.", ParameterIs.Required)]
	[Param(3, typeof(double), "delaySec", "An optional time to wait before setting the visibility, in seconds.", ParameterIs.Optional)]
	[Param(4, typeof(string), "sceneName", "An optional name of the OBS scene containing the source to change.", ParameterIs.Optional)]
	public class SetObsSourceVisibilityFunction : DndFunction
	{
		public delegate void SetObsSourceVisibilityEventHandler(object sender, SetObsSourceVisibilityEventArgs ea);
		public static event SetObsSourceVisibilityEventHandler RequestSetObsSourceVisibility;

		public static void OnRequestSetObsSourceVisibility(string sceneName, string sourceName, bool visible, double delaySec)
		{
			SetObsSourceVisibilityEventArgs ea = new SetObsSourceVisibilityEventArgs(sourceName, sceneName, visible, delaySec);
			RequestSetObsSourceVisibility?.Invoke(null, ea);
		}
		public override string Name => "SetObsSourceVisibility";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 2, 4);

			string sourceName = evaluator.Evaluate<string>(args[0]);
			string sceneName = null;
			double delaySec = 0;
			bool visible;
			visible = Expressions.GetBool(args[1]);
			if (args.Count > 2)
			{
				delaySec = Expressions.GetDouble(args[2].Trim());
				if (args.Count > 3)
					sceneName = evaluator.Evaluate<string>(args[3]);
			}

			OnRequestSetObsSourceVisibility(sceneName, sourceName, visible, delaySec);

			return null;
		}
	}
}
