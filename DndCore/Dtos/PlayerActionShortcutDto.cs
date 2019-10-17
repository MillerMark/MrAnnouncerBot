using System;
using System.Linq;

namespace DndCore
{
	public class PlayerActionShortcutDto
	{
		public string player { get; set; }
		public string type { get; set; }
		public string vantageMod { get; set; }
		public string rollMod { get; set; }
		public string commands { get; set; }
		public string name { get; set; }
		public string addDice { get; set; }
		public string addDiceOnHit { get; set; }
		public string addDiceOnHitMessage { get; set; }
		public string addDiceTitle { get; set; }
		public string time { get; set; }
		public string magic { get; set; }
		public string dieStr { get; set; }
		public string plusModifier { get; set; }
		public string minDamage { get; set; }
		public string effect { get; set; }
		public string hue { get; set; }
		public string brightness { get; set; }
		public string saturation { get; set; }
		public string scale { get; set; }
		public string rotation { get; set; }
		public string degreesOffset { get; set; }
		public string playToEndOnExpire { get; set; }
		public string flipHorizontal { get; set; }
		public string moveUpDown { get; set; }
		public string moveLeftRight { get; set; }
		public string fade { get; set; }
		public string opacity { get; set; }
		public string minSlotLevel { get; set; }
		public string startSound { get; set; }
		public string endSound { get; set; }
		public string description { get; set; }
		public string trailingEffects { get; set; }
		public string dieRollEffects { get; set; }

		public PlayerActionShortcutDto()
		{
			player = "";
			type = "";
			vantageMod = "";
			rollMod = "";
			commands = "";
			name = "";
			addDice = "";
			addDiceOnHit = "";
			addDiceOnHitMessage = "";
			addDiceTitle = "";
			time = "";
			magic = "";
			dieStr = "";
			plusModifier = "";
			minDamage = "";
			effect = "";
			hue = "";
			brightness = "";
			saturation = "";
			scale = "";
			rotation = "";
			degreesOffset = "";
			playToEndOnExpire = "";
			flipHorizontal = "";
			moveUpDown = "";
			moveLeftRight = "";
			fade = "";
			opacity = "";
			minSlotLevel = "";
			startSound = "";
			endSound = "";
			description = "";
			trailingEffects = "";
			dieRollEffects = "";
		}
	}
}
