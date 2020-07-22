using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public class InGameCommand
	{
		public string Command { get; set; }
		public List<InGameCreature> Creatures { get; set; }
		public InGameCommand(string command, List<InGameCreature> creatures)
		{
			Command = command;
			Creatures = creatures;
		}
		public InGameCommand()
		{

		}
	}
}
