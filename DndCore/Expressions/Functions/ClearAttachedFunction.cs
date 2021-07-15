using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Clears the attached effect for the active spell on the active player's TaleSpire mini.")]
	[Param(1, typeof(float), "secondsDelay", "The seconds to wait before clearing the attached effect.", ParameterIs.Optional)]
	public class ClearAttachedFunction : DndFunction
	{
		public static event SpellEffectEventHandler ClearAttached;
		public override string Name { get; set; } = "ClearAttached";

		static void OnClearAttached(string spellId, string taleSpireId, float secondsDelayStart)
		{
			ClearAttached?.Invoke(null, new SpellEffectEventArgs(null, spellId, taleSpireId, EffectLocation.CreatureBase, 0, secondsDelayStart));
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 0, 1);
			
			string spellId = null;
			if (spell != null)
				spellId = spell.ID;
			else
			{
				CreaturePlusModId creaturePlusModId = Expressions.GetCustomData<CreaturePlusModId>(evaluator.Variables);
				if (creaturePlusModId != null)
					spellId = creaturePlusModId.Guid;
			}

			if (player == null || spellId == null)
				return null;

			float secondsDelayStart = 0;

			if (args.Count > 0)
				float.TryParse(args[0], out secondsDelayStart);

			OnClearAttached(spellId, player.taleSpireId, secondsDelayStart);

			return null;
		}
	}
}
