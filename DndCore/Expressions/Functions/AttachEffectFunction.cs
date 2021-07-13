using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Attaches the specified spell effect to the active player's TaleSpire mini.")]
	[Param(1, typeof(string), "spellEffectName", "The name of the known effect or Prefab to attach.", ParameterIs.Required)]
	[Param(2, typeof(float), "secondsDelay", "The seconds to delay until the spell is attached.", ParameterIs.Optional)]
	[Param(3, typeof(float), "enlargeTime", "The seconds to enlarge this effect.", ParameterIs.Optional)]
	[Param(4, typeof(float), "lifeTime", "The lifetime until the effect is cleared.", ParameterIs.Optional)]
	[Param(5, typeof(float), "shrinkTime", "The seconds to shrink this effect when it stops.", ParameterIs.Optional)]
	[Param(6, typeof(float), "rotation", "The degrees to rotate this effect.", ParameterIs.Optional)]
	public class AttachEffectFunction : DndFunction
	{
		public static event SpellEffectEventHandler AttachEffect;
		public override string Name { get; set; } = "AttachEffect";

		static void OnAttachChargingEffect(string effectName, string iD, string taleSpireId, float secondsDelayStart, float enlargeTime, float lifeTime, float shrinkTime, float rotationDegrees)
		{
			AttachEffect?.Invoke(null, new SpellEffectEventArgs(effectName, iD, taleSpireId, EffectLocation.CreatureBase, lifeTime, secondsDelayStart, enlargeTime, shrinkTime, rotationDegrees));
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1, 6);
			float lifeTime = 0;
			float shrinkTime = 0;
			float rotationDegrees = 0;
			if (player != null && spell != null)
			{
				string effectName = Expressions.GetStr(args[0]);
				float secondsDelayStart = 0;
				float enlargeTime = 0;
				if (args.Count > 1)
				{
					float.TryParse(args[1], out secondsDelayStart);
					if (args.Count > 2)
					{
						float.TryParse(args[2], out enlargeTime);
						if (args.Count > 3)
						{
							float.TryParse(args[3], out lifeTime);
							if (args.Count > 4)
							{
								float.TryParse(args[4], out shrinkTime);
								if (args.Count > 5)
									float.TryParse(args[5], out rotationDegrees);
							}
						}
					}
				}

				OnAttachChargingEffect(effectName, spell.ID, player.taleSpireId, secondsDelayStart, enlargeTime, lifeTime, shrinkTime, rotationDegrees);
			}

			return null;
		}
	}
}
