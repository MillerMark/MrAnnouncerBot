using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Attaches the specified spell effect to the active player's TaleSpire mini.")]
	[Param(1, typeof(string), "effectName", "The name of the known effect or Prefab to attach.", ParameterIs.Required)]
	[Param(2, typeof(ProjectileKind), "projectileKind", "The kind of projectiles to launch (one of ToVolume, DistributeAmongAllTargets, or EachTarget).", ParameterIs.Optional)]
	[Param(3, typeof(int), "projectileCount", "The number of projectiles to launch.", ParameterIs.Optional)]
	[Param(4, typeof(float), "speed", "The average speed (ft/sec) of the projectile. Easing will be added.", ParameterIs.Optional)]
	[Param(5, typeof(FireCollisionEventOn), "fireCollisionEventOn", "When to fire the collision events (one of FirstImpact, EachImpact, LastImpact).", ParameterIs.Optional)]
	[Param(6, typeof(float), "launchTimeVariance", "The average time between launches.", ParameterIs.Optional)]
	[Param(7, typeof(float), "targetVariance", "The average distance variance to the target for the projectile (0 means a precise hit).", ParameterIs.Optional)]
	[Param(8, typeof(ProjectileSizeOption), "projectileSize", "Determines how the projectile changes size in flight.", ParameterIs.Optional)]
	[Param(9, typeof(float), "projectileSizeMultiplier", "Determines how much (scaling) of the projectile size option to apply.", ParameterIs.Optional)]
	public class LaunchProjectileFunction : DndFunction
	{
		public static event ProjectileEffectEventHandler LaunchProjectile;
		public override string Name { get; set; } = "LaunchProjectile";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 3, 10);
			if (player == null || spell == null)
				return null;
			string effectName = Expressions.GetStr(args[0]);
			ProjectileKind projectileKind = ProjectileKind.EachTarget;
			int projectileCount = 1;
			float speed = 20;
			FireCollisionEventOn fireCollisionEventOn = FireCollisionEventOn.EachImpact;
			float launchTimeVariance = 0;
			float targetVariance = 0;
			ProjectileSizeOption projectileSize = ProjectileSizeOption.ConstantSize;
			float projectileSizeMultiplier = 1;

			if (args.Count > 1)
			{
				projectileKind = Expressions.Get<ProjectileKind>(args[1]);
				if (args.Count > 2)
				{
					int.TryParse(args[2], out projectileCount);
					if (args.Count > 3)
					{
						float.TryParse(args[3], out speed);

						if (args.Count > 4)
						{
							fireCollisionEventOn = Expressions.Get<FireCollisionEventOn>(args[4]);
							if (args.Count > 5)
							{
								float.TryParse(args[5], out launchTimeVariance);
								if (args.Count > 6)
								{
									float.TryParse(args[6], out targetVariance);
									if (args.Count > 7)
									{
										projectileSize = Expressions.Get<ProjectileSizeOption>(args[7]);
										if (args.Count > 8)
											float.TryParse(args[8], out projectileSizeMultiplier);
									}
								}
							}
						}
					}
				}
			}

			ProjectileEffectEventArgs ea = new ProjectileEffectEventArgs(effectName, spell.ID, player.taleSpireId, speed, projectileCount, projectileKind, fireCollisionEventOn, launchTimeVariance, targetVariance, target, projectileSize, projectileSizeMultiplier);
			LaunchProjectile?.Invoke(null, ea);

			return null;
		}
	}
}
