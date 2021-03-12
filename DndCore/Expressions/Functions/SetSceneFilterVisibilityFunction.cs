using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Sets visibility for the specified filter in the specified scene in OBS.")]
	[Param(1, typeof(string), "sourceName", "The name of source in OBS.", ParameterIs.Required)]
	[Param(2, typeof(string), "filterName", "The name of the filter to set visibility for.", ParameterIs.Required)]
	[Param(3, typeof(bool), "filterEnabled", "true to show; false to hide.", ParameterIs.Required)]
	[Param(4, typeof(int), "delayMs", "The time to wait, in ms, before changing visibility.", ParameterIs.Optional)]

	public class SetSceneFilterVisibilityFunction : DndFunction
	{
		public static event ObsSceneFilterEventHandler RequestSetObsSceneFilterVisibility;
		
		public static void OnRequestSetObsSceneFilterVisibility(object sender, ObsSceneFilterEventArgs ea)
		{
			RequestSetObsSceneFilterVisibility?.Invoke(sender, ea);
		}

		public override string Name => "SetSceneFilterVisibility";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 3, 4);

			string sourceName = Expressions.GetStr(args[0], player);
			string filterName = Expressions.GetStr(args[1]);
			bool filterEnabled = Expressions.GetBool(args[2]);
			int delayMs = 0;
			if (args.Count > 3)
				delayMs = Expressions.GetInt(args[3]);

			OnRequestSetObsSceneFilterVisibility(null, new ObsSceneFilterEventArgs(sourceName, filterName, filterEnabled, delayMs));
			return null;
		}
	}
}
