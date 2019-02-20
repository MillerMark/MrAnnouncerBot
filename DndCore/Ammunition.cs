using System;
using System.Linq;

namespace DndCore
{
	public class Ammunition : Item
	{
		public static Ammunition buildBlowgunNeedlePack()
		{
			Ammunition blowDart = new Ammunition();
			blowDart.name = "Blow Needle";
			blowDart.costValue = 1;
			blowDart.weight = 1;
			blowDart.count = 50;
			return blowDart;
		}
	}
}
