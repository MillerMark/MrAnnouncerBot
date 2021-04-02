using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DeltaTargetFunction : DndFunction
	{
		public override string Name { get; set; } = "DeltaTarget";

		public static event PropertyChangeEventHandler RequestPropertyChange;
		
		public static void OnRequestPropertyChange(object sender, PropertyChangeEventArgs ea)
		{
			RequestPropertyChange?.Invoke(sender, ea);
		}

		void OnRequestPropertyChange(Creature creature, string propertyName, double deltaValue)
		{
			PropertyChangeEventArgs ea = new PropertyChangeEventArgs(creature, propertyName, deltaValue);
			OnRequestPropertyChange(null, ea);
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 2);

			string propertyName = Expressions.GetStr(args[0], player, target, spell, dice);
			double deltaValue = Expressions.GetDouble(args[1], player, target, spell, dice);

			if (target == null)
			{
				// Make sure this Spell has values in the MinTargetsToCast and MaxTargetsToCast columns.
				System.Diagnostics.Debugger.Break();
				return null;
			}

			foreach (Creature creature in target.Creatures)
				OnRequestPropertyChange(creature, propertyName, deltaValue);

			if (player != null)
				foreach (int playerId in target.PlayerIds)
				{
					Character playerFromId = player.Game.GetPlayerFromId(playerId);
					if (playerFromId != null)
						OnRequestPropertyChange(playerFromId, propertyName, deltaValue);
				}


			return null;
		}
	}
}
