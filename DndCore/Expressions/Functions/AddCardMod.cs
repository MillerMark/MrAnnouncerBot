using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified mod to the next roll.")]
	[Param(1, typeof(string), "modType", "The type of card mod to add (e.g., TotalDamage, TotalScore, or TotalScorePlusDamage).", ParameterIs.Required)]
	[Param(2, typeof(int), "valueOffset", "The value of the mod (e.g., -1, 1, 2, 3, etc.).", ParameterIs.Required)]
	[Param(3, typeof(int), "multiplier", "The multiplier of the mod (e.g., 1, 2, etc.).", ParameterIs.Optional)]
	[Param(4, typeof(string), "userName", "The name of the user responsible for this mod.", ParameterIs.Optional)]
	public class AddCardMod : DndFunction
	{
		public override string Name { get; set; } = "AddCardMod";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 2, 4);

			CardModType modType = DndUtils.ToCardModType(args[0].Trim());
			
			int valueOffset;
			double multiplier = 1;
			int.TryParse(args[1], out valueOffset);

			string userName = "";
			if (args.Count > 2)
			{
				double.TryParse(args[2], out multiplier);
				if (args.Count > 3)
				{
					userName = Expressions.GetStr(args[3].Trim());
				}
			}

			//CreaturePlusModId recipient = Expressions.GetCustomData<CreaturePlusModId>(evaluator.Variables);
			if (SystemVariables.DiceRoll != null)
				SystemVariables.DiceRoll.CardModifiers.Add(new CardModifier(player.SafeId, userName, valueOffset, multiplier));
			return null;
		}
	}
}
