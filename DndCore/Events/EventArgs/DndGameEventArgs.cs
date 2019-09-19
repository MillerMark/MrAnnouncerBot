using System;
using System.Linq;

namespace DndCore
{
	public class DndGameEventArgs : EventArgs
	{
		public DndGame Game { get; set; }

		public DndGameEventArgs(DndGame game)
		{
			Game = game;
		}

		public DndGameEventArgs()
		{

		}
	}
}
