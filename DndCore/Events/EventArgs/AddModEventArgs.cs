using System;
using System.Linq;

namespace DndCore
{
	public class AddModEventArgs : DispelMagicEventArgs
	{
		public AddModEventArgs(CreaturePlusModId recipient, string modType, int modOffset, double modMultiplier = 1): base(recipient)
		{
			Multiplier = modMultiplier;
			Offset = modOffset;
			Type = modType;
		}
		public string Type { get; set; }
		public int Offset { get; set; }
		public double Multiplier { get; set; }
	}

}
