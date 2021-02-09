using System;
using System.Linq;

namespace DndCore
{
	public class PlayerShowStateEventArgs : EventArgs
	{

		public PlayerShowStateEventArgs(Creature player, string message, string fillColor, string outlineColor, int delayMs)
		{
			DelayMs = delayMs;
			Player = player;
			OutlineColor = outlineColor;
			FillColor = fillColor;
			Message = message;
		}

		public string Message { get; set; }
		public string FillColor { get; set; }
		public string OutlineColor { get; set; }
		public Creature Player { get; set; }
		public int DelayMs { get; set; }
	}
}