using System;
using System.Linq;

namespace DndCore
{
	public class PlayerShowStateEventArgs : EventArgs
	{

		public PlayerShowStateEventArgs(Character player, string message, string fillColor, string outlineColor)
		{
			Player = player;
			OutlineColor = outlineColor;
			FillColor = fillColor;
			Message = message;
		}

		public string Message { get; set; }
		public string FillColor { get; set; }
		public string OutlineColor { get; set; }
		public Character Player { get; set; }
	}
}