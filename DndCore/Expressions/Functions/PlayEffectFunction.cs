using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Plays the specified spell effect over the active player's TaleSpire mini.")]
	[Param(1, typeof(string), "spellEffectName", "The name of the known effect or Prefab to create.", ParameterIs.Required)]
	[Param(2, typeof(float), "duration", "The duration, in seconds, for this spell effect to last before destroying it.", ParameterIs.Optional)]
	[Param(3, typeof(EffectLocation), "effectLocation", "The location to play this effect. One of CreatureBase (default), SpellCast, LastTargetPosition, AtWall, AtCollision, AtCollisionTarget (using the *intended* target position), AtCollisionBase (hitting the floor beneath the intended target), MoveableTarget, or MoveableSpellCast.", ParameterIs.Optional)]
	[Param(4, typeof(float), "secondsDelay", "The seconds to delay playing this effect.", ParameterIs.Optional)]
	[Param(5, typeof(float), "enlargeTime", "The seconds to enlarge this effect.", ParameterIs.Optional)]
	[Param(6, typeof(float), "shrinkTime", "The seconds to shrink this effect when it stops.", ParameterIs.Optional)]
	[Param(7, typeof(float), "rotation", "The degrees to rotate this effect.", ParameterIs.Optional)]
	[Param(8, typeof(float), "wallLength", "Length of the wall (for walls).", ParameterIs.Optional)]
	public class PlayEffectFunction : DndFunction
	{
		public static event SpellEffectEventHandler PlayEffect;
		public override string Name { get; set; } = "PlayEffect";

		static void OnPlayKnownEffect(string effectName, string spellId, string taleSpireId, float lifeTime, EffectLocation effectLocation, float secondsDelayStart, float enlargeTime, float shrinkTime, float rotation, float wallLength)
		{
			PlayEffect?.Invoke(null, new SpellEffectEventArgs(effectName, spellId, taleSpireId, effectLocation, lifeTime, secondsDelayStart, enlargeTime, shrinkTime, rotation, wallLength));
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1, 8);
			if (player == null)
				return null;

			string spellId;
			if (spell == null)
				spellId = Guid.NewGuid().ToString();
			else
				spellId = spell.ID;

			string effectName = Expressions.GetStr(args[0]);
			float lifeTime = 0;
			float secondsDelayStart = 0;
			float enlargeTime = 0;
			float shrinkTime = 0;
			float rotation = 0;
			float wallLength = 0;
			EffectLocation effectLocation = EffectLocation.CreatureBase;
			if (args.Count > 1)
			{
				lifeTime = (float)Expressions.GetDouble(args[1]);
				if (args.Count > 2)
				{
					effectLocation = Expressions.Get<EffectLocation>(args[2]);
					if (args.Count > 3)
					{
						secondsDelayStart = Expressions.GetFloat(args[3]);
						if (args.Count > 4)
						{
							enlargeTime = Expressions.GetFloat(args[4]);
							if (args.Count > 5)
							{
								shrinkTime = Expressions.GetFloat(args[5]);
								if (args.Count > 6)
								{
									rotation = Expressions.GetFloat(args[6]);
									if (args.Count > 7)
										wallLength = Expressions.GetFloat(args[7]);
								}
							}
						}
					}
				}
			}

			OnPlayKnownEffect(effectName, spellId, player.taleSpireId, lifeTime, effectLocation, secondsDelayStart, enlargeTime, shrinkTime, rotation, wallLength);

			return null;
		}
	}
}
