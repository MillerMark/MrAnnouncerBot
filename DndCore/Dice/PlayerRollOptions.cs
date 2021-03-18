using System;
using System.Linq;

namespace DndCore
{
	public class PlayerRollOptions
	{
		public int PlayerID { get; set; }
		public double Scale { get; set; } = 1;
		public string Inspiration { get; set; }
		public VantageKind VantageKind { get; set; }
		public PlayerRollOptions(int playerId, VantageKind vantageKind, string inspiration)
		{
			PlayerID = playerId;
			VantageKind = vantageKind;
			Inspiration = inspiration;
		}
	}
}
