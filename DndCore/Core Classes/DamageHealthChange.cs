using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class DamageHealthChange
	{
		public List<int> PlayerIds = new List<int>();
		public int DamageHealth { get; set; }
		public bool IsTempHitPoints { get; set; }
		public DamageHealthChange()
		{

		}
	}
}
