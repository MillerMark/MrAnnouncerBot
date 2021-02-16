using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public interface IGetUserName
	{
		string GetUserName();
	}

	[Tooltip("Returns the name of the user associated with the specified card.")]
	[Param(1, typeof(object), "card", "The CardDto to check.", ParameterIs.Required)]
	public class GetUserName : DndFunction
	{
		public static event AddModEventHandler RequestAddMod;
		public override string Name { get; set; } = "GetUserName";
		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);
			IGetUserName iGetUserName = Expressions.Get<IGetUserName>(args[0]);
			if (iGetUserName != null)
				return iGetUserName.GetUserName();
			return null;
		}
	}
}
