using System;
using System.Linq;

namespace DHDM
{
	public abstract class ReleaseTrigger
	{
		ReleaseTrigger(string name, string description)
		{
			Description = description;
			Name = name;
		}

		public abstract bool IsReleased();
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
