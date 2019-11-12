using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class SpellGroup
	{
		public string Name { get; set; }
		public int TotalCharges { get; set; }
		public int ChargesUsed { get; set; }
		public List<SpellDataItem> SpellDataItems { get; set; } = new List<SpellDataItem>();
		public SpellGroup()
		{

		}
	}
}
