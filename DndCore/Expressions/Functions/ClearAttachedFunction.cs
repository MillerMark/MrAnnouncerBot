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

			
			float secondsDelayStart = 0;

			if (args.Count > 0)
				float.TryParse(args[0], out secondsDelayStart);

			Magic magic = Expressions.GetCustomData<Magic>(evaluator.Variables);
			if (magic != null && target != null && target.Creatures != null && target.Creatures.Count > 0)
				foreach (Creature creature in target.Creatures)
					OnClearAttached(magic.Id, creature.taleSpireId, secondsDelayStart);

			string spellId = evaluator.GetSpellId(spell);
			
			if (player == null || spellId == null)
				return null;

			OnClearAttached(spellId, player.taleSpireId, secondsDelayStart);

			return null;
		}
	}
}
