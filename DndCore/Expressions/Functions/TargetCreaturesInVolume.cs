using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Targets all creatures in the most recently targeted volume.")]
	[Param(1, typeof(WhatSide), "whatSide", "The kind of creature to target (e.g., All, Enemy, Friendly, Neutral).", ParameterIs.Optional)]
	public class TargetCreaturesInVolume : DndFunction
	{
		public override string Name { get; set; } = "TargetCreaturesInVolume";
		public static event WhatSideEventHandler TargetCreaturesInVolumeRequest;
		
		public static void OnTargetCreaturesInVolumeRequest(object sender, WhatSideEventArgs ea)
		{
			TargetCreaturesInVolumeRequest?.Invoke(sender, ea);
		}

		public override object Evaluate(
			List<string> args,
			ExpressionEvaluator evaluator,
			Creature player,
			Target target,
			CastedSpell spell,
			RollResults dice = null)
		{
			ExpectingArguments(args, 0, 1);

			WhatSide whatSide = WhatSide.All;
			if (args.Count > 0)
			{
				string whatSideStr = args[0].Trim();
				string[] parts = whatSideStr.Split('|');  // Union enum elements, e.g., "Friendly | Neutral"
				whatSide = WhatSide.None;
				foreach (string part in parts)
					whatSide |= DndUtils.ToWhatSide(part);
			}

			OnTargetCreaturesInVolumeRequest(player, new WhatSideEventArgs(whatSide));

			return null;
		}
	}
}
