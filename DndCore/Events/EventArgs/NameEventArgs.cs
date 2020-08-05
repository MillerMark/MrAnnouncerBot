using System;
using System.Linq;

namespace DndCore
{
	public class NameEventArgs : EventArgs
	{
		public string Name { get; set; }
		public NameEventArgs(string name)
		{
			Name = name;
		}
		public NameEventArgs()
		{

		}
	}
}
