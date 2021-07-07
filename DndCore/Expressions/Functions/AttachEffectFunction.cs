using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Attaches the specified spell effect to the active player's TaleSpire mini.")]
	[Param(1, typeof(string), "spellEffectName", "The name of the known effect or Prefab to attach.", ParameterIs.Required)]
	[Param(2, typeof(float), "secondsDelay", "The seconds to delay until the spell is attached.", ParameterIs.Required)]
	public class AttachEffectFunction : DndFunction
	{
		public static event SpellEffectEventHandler AttachEffect;
		public override string Name { get; set; } = "AttachEffect";

		static void OnAttachChargingEffect(string effectName, string iD, string taleSpireId, float secondsDelayStart)
		{
			AttachEffect?.Invoke(null, new SpellEffectEventArgs(effectName, iD, taleSpireId, EffectLocation.ActiveCreaturePosition, 0, secondsDelayStart));
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1, 2);
			if (player != null && spell != null)
			{
				string effectName = Expressions.GetStr(args[0]);
				float secondsDelayStart = 0;
				if (args.Count > 1)
					float.TryParse(args[1], out secondsDelayStart);

				OnAttachChargingEffect(effectName, spell.ID, player.taleSpireId, secondsDelayStart);
			}

			return null;
		}
	}
}
