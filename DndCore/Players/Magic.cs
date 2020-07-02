using System;
using System.Collections.Generic;

namespace DndCore
{
	public class Magic
	{
		public event MagicEventHandler OnDispel;
		public Magic(Creature caster, string magicItemName, object data1, object data2, object data3, object data4, object data5, object data6, object data7, object data8)
		{
			Data1 = data1;
			Data2 = data2;
			Data3 = data3;
			Data4 = data4;
			Data5 = data5;
			Data6 = data6;
			Data7 = data7;
			Data8 = data8;
			MagicItemName = magicItemName;
			Caster = caster;
		}

		public Creature Caster { get; set; }
		List<Creature> targets = new List<Creature>();
		public void AddTarget(Creature target)
		{
			targets.Add(target);
		}
		public string MagicItemName { get; set; }
		MagicItem magicItem;
		public MagicItem MagicItem
		{
			get
			{
				if (magicItem == null)
					magicItem = AllMagicItems.Get(MagicItemName);
				return magicItem;
			}
		}

		// TODO: Call this before we trigger any Magic events.
		public void SetMagicVariables()
		{
			
		}

		public object Data1 { get; set; }
		public object Data2 { get; set; }
		public object Data3 { get; set; }
		public object Data4 { get; set; }
		public object Data5 { get; set; }
		public object Data6 { get; set; }
		public object Data7 { get; set; }
		public object Data8 { get; set; }
	}
}
