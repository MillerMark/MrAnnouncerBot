using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified sound effect for the next spell that is cast.")]
	[Param(1, typeof(string), "fileName", "The name of the mp3 sound file (located in wwwroot\\GameDev\\Assets\\DragonH\\SoundEffects or a sub folder) to play.", ParameterIs.Required, CompletionProviderNames.SoundFile)]
	[Param(2, typeof(int), "timeOffsetMs", "The amount of time to wait (in ms) until the sound file is played.", ParameterIs.Optional)]
	public class AddSpellCastSound : DndFunction
	{
		public override string Name { get; set; } = "AddSpellCastSound";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1, 2);
			if (player != null)
			{
				string fileName = Expressions.GetStr(args[0], player, target, spell);

				int timeOffsetMs = 0;
				if (args.Count > 1)
					timeOffsetMs = Expressions.GetInt(args[1], player, target, spell);

				player.AddSpellCastSoundEffect(fileName, timeOffsetMs);
			}

			return null;
		}
	}
}
