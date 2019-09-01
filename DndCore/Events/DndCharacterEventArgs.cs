using System;
using System.Linq;

namespace DndCore
{
	public class DndCharacterEventArgs : EventArgs
	{
		public Character Player { get; set; }
		public DndCharacterEventArgs()
		{

		}
	}
}
