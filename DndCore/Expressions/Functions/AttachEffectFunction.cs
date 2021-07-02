using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Attaches the specified spell effect to the active player's TaleSpire mini.")]
	[Param(1, typeof(string), "spellEffectName", "The name of the known effect or Prefab to attach.", ParameterIs.Required)]
	public class AttachEffectFunction : DndFunction
	{
		public static event SpellEffectEventHandler AttachEffect;
		public override string Name { get; set; } = "AttachEffect";

		static void OnAttachChargingEffect(string effectName, string iD, string taleSpireId)
		{
			AttachEffect?.Invoke(null, new SpellEffectEventArgs(effectName, iD, taleSpireId));
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1);
			if (player != null)
			{
				string effectName = Expressions.GetStr(args[0]);
				if (spell != null)
					OnAttachChargingEffect(effectName, spell.ID, player.taleSpireId);
			}

			return null;
		}
	}
}
