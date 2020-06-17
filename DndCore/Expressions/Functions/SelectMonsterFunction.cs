using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Selects and returns the monster.")]
	[Param(1, typeof(CreatureKinds), "creatureKindFilter", "The creature kinds (Aberration, Beast, Celestial, Construct, Dragon, Elemental, Fey, Fiend, Giant, Humanoid, Monstrosity, Ooze, Plant, Undead, or AllCreatures) to limit choices to.", ParameterIs.Optional)]
	[Param(2, typeof(int), "maxChallengeRating", "The maximum challenge rating for the monster.", ParameterIs.Optional)]
	[Param(3, typeof(string), "speedLimitations", "Any speed limitations (flying, swimming, etc.) for the monster.", ParameterIs.Optional)]
	public class SelectMonsterFunction : DndFunction
	{
		public static event SelectMonsterEventHandler RequestSelectMonster;

		public static void OnRequestSelectMonster(SelectMonsterEventArgs ea)
		{
			RequestSelectMonster?.Invoke(null, ea);
		}

		public override string Name { get; set; } = "SelectMonster";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0, 3);

			SelectMonsterEventArgs ea = new SelectMonsterEventArgs(player);
			if (args.Count > 0)
			{
				ea.CreatureKindFilter = DndUtils.ToCreatureKind(args[0]);
				if (args.Count > 1)
				{
					ea.MaxChallengeRating = Expressions.GetDouble(args[1], player, target, spell);
					if (args.Count > 2)
					{
						string speedLimitations = Expressions.GetStr(args[2], player, target, spell);
						if (speedLimitations != null)
						{
							string[] limitations = speedLimitations.Split(',');
							foreach (string limitation in limitations)
							{
								switch (limitation.Trim())
								{
									case "flying":
										ea.MaxFlyingSpeed = 0;
										break;
									case "swimming":
										ea.MaxSwimmingSpeed = 0;
										break;
								}
							}
						}
					}
				}
			}

			OnRequestSelectMonster(ea);

			return ea.Monster;
		}
	}
}
