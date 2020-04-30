using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public enum TargetKind
	{
		Player,
		Weapon
	}
	
	public class TargetEventArgs : EventArgs
	{
		public Target Target { get; set; }
		public Character Player { get; set; }
		public bool AllowSelf { get; set; }
		public TargetEventArgs()
		{
			
		}
	}
	public delegate void TargetEventHandler(TargetEventArgs ea);
	public class SelectTargetFunction : DndFunction
	{
		public static event TargetEventHandler RequestSelectTarget;
		public static void OnRequestSelectTarget(TargetEventArgs ea)
		{
			RequestSelectTarget?.Invoke(ea);
		}
		public override string Name { get; set; } = "SelectTarget";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0);

			TargetEventArgs ea = new TargetEventArgs();
			ea.Player = player;
			ea.Target = target;
			OnRequestSelectTarget(ea);
			player.ActiveTarget = ea.Target;

			return null;
		}
	}
}
